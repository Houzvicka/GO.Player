using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Protection;
using Windows.Media.Protection.PlayReady;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Xaml.Controls;

namespace HBO.UWP.Player.Helpers
{
    public class PRH
    {
        private AdaptiveMediaSource ams = null;
        private MediaProtectionManager protectionManager = null;
        private string playReadyLicenseUrl = $"https://lic.drmtoday.com/license-proxy-headerauth/drmtoday/RightsManager.asmx";
        private string playReadyChallengeCustomData = "assetId={}&variantId={}";

        private const uint MSPR_E_CONTENT_ENABLING_ACTION_REQUIRED = 0x8004B895;

        async public void InitializeAdaptiveMediaSource(System.Uri uri, MediaElement m )
        {
            AdaptiveMediaSourceCreationResult result = await AdaptiveMediaSource.CreateFromUriAsync(uri);
            if (result.Status == AdaptiveMediaSourceCreationStatus.Success)
            {
                ams = result.MediaSource;
                SetUpProtectionManager(ref m);
                m.SetMediaStreamSource(ams);
            }
            else
            {
                // Error handling
            }
        }

        private void SetUpProtectionManager(ref MediaElement mediaElement)
        {
            protectionManager = new MediaProtectionManager();

            protectionManager.ComponentLoadFailed +=
                new ComponentLoadFailedEventHandler(ProtectionManager_ComponentLoadFailed);

            protectionManager.ServiceRequested +=
                new ServiceRequestedEventHandler(ProtectionManager_ServiceRequested);

            PropertySet cpSystems = new PropertySet();

            cpSystems.Add(
                "{F4637010-03C3-42CD-B932-B48ADF3A6A54}",
                "Windows.Media.Protection.PlayReady.PlayReadyWinRTTrustedInput");

            protectionManager.Properties.Add("Windows.Media.Protection.MediaProtectionSystemIdMapping", cpSystems);

            protectionManager.Properties.Add(
                "Windows.Media.Protection.MediaProtectionSystemId",
                "{F4637010-03C3-42CD-B932-B48ADF3A6A54}");

            protectionManager.Properties.Add(
                "Windows.Media.Protection.MediaProtectionContainerGuid",
                "{9A04F079-9840-4286-AB92-E65BE0885F95}");

            mediaElement.ProtectionManager = protectionManager;
        }

        private void ProtectionManager_ComponentLoadFailed(
            MediaProtectionManager sender,
            ComponentLoadFailedEventArgs e)
        {
            e.Completion.Complete(false);
        }

        private async void ProtectionManager_ServiceRequested(
            MediaProtectionManager sender,
            ServiceRequestedEventArgs e)
        {
            if (e.Request is PlayReadyIndividualizationServiceRequest)
            {
                PlayReadyIndividualizationServiceRequest IndivRequest =
                    e.Request as PlayReadyIndividualizationServiceRequest;

                bool bResultIndiv = await ReactiveIndivRequest(IndivRequest, e.Completion);
            }
            else if (e.Request is PlayReadyLicenseAcquisitionServiceRequest)
            {
                PlayReadyLicenseAcquisitionServiceRequest licenseRequest =
                    e.Request as PlayReadyLicenseAcquisitionServiceRequest;

                LicenseAcquisitionRequest(
                    licenseRequest,
                    e.Completion,
                    playReadyLicenseUrl,
                    playReadyChallengeCustomData);
            }
        }

        async Task<bool> ReactiveIndivRequest(
            PlayReadyIndividualizationServiceRequest IndivRequest,
            MediaProtectionServiceCompletion CompletionNotifier)
        {
            bool bResult = false;
            Exception exception = null;

            try
            {
                await IndivRequest.BeginServiceRequest();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (exception == null)
                {
                    bResult = true;
                }
                else
                {
                    COMException comException = exception as COMException;
                    if (comException != null && comException.HResult == MSPR_E_CONTENT_ENABLING_ACTION_REQUIRED)
                    {
                        IndivRequest.NextServiceRequest();
                    }
                }
            }

            if (CompletionNotifier != null) CompletionNotifier.Complete(bResult);
            return bResult;
        }

        async void ProActiveIndivRequest()
        {
            PlayReadyIndividualizationServiceRequest indivRequest = new PlayReadyIndividualizationServiceRequest();
            bool bResultIndiv = await ReactiveIndivRequest(indivRequest, null);
        }

        async void LicenseAcquisitionRequest(
    PlayReadyLicenseAcquisitionServiceRequest licenseRequest,
    MediaProtectionServiceCompletion CompletionNotifier,
    string Url,
    string ChallengeCustomData)
        {
            bool bResult = false;
            string ExceptionMessage = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Url))
                {
                    if (!string.IsNullOrEmpty(ChallengeCustomData))
                    {
                        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                        byte[] b = encoding.GetBytes(ChallengeCustomData);
                        licenseRequest.ChallengeCustomData = Convert.ToBase64String(b, 0, b.Length);
                    }

                    PlayReadySoapMessage soapMessage = licenseRequest.GenerateManualEnablingChallenge();

                    byte[] messageBytes = soapMessage.GetMessageBody();
                    HttpContent httpContent = new ByteArrayContent(messageBytes);

                    IPropertySet propertySetHeaders = soapMessage.MessageHeaders;

                    foreach (string strHeaderName in propertySetHeaders.Keys)
                    {
                        string strHeaderValue = propertySetHeaders[strHeaderName].ToString();

                        if (strHeaderName.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                        {
                            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(strHeaderValue);
                        }
                        else
                        {
                            httpContent.Headers.Add(strHeaderName.ToString(), strHeaderValue);
                        }
                    }

                    CommonLicenseRequest licenseAcquision = new CommonLicenseRequest();

                    HttpContent responseHttpContent =
                        await licenseAcquision.AcquireLicenseAsync(new Uri(Url), httpContent);

                    if (responseHttpContent != null)
                    {
                        Exception exResult = licenseRequest.ProcessManualEnablingResponse(
                                                 await responseHttpContent.ReadAsByteArrayAsync());

                        if (exResult != null)
                        {
                            throw exResult;
                        }
                        bResult = true;
                    }
                    else
                    {
                        ExceptionMessage = licenseAcquision.GetLastErrorMessage();
                    }
                }
                else
                {
                    await licenseRequest.BeginServiceRequest();
                    bResult = true;
                }
            }
            catch (Exception e)
            {
                ExceptionMessage = e.Message;
            }

            CompletionNotifier.Complete(bResult);
        }
    }

    public class CommonLicenseRequest
    {
        private string lastErrorMessage;
        private HttpClient httpClient;
        public string GetLastErrorMessage()
        {
            return lastErrorMessage;
        }
        public CommonLicenseRequest(HttpClient client = null)
        {
            if (client == null)
                httpClient = new HttpClient();
            else
                httpClient = client;
            lastErrorMessage = string.Empty;
        }
        /// <summary>
        /// Invoked to acquire the PlayReady license.
        /// </summary>
        /// <param name="licenseServerUri">License Server URI to retrieve the PlayReady license.</param>
        /// <param name="httpRequestContent">HttpContent including the Challenge transmitted to the PlayReady server.</param>
        public async virtual Task<HttpContent> AcquireLicenseAsync(Uri licenseServerUri, HttpContent httpRequestContent)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("msprdrm_server_redirect_compat", "false");
                httpClient.DefaultRequestHeaders.Add("msprdrm_server_exception_compat", "false");

                HttpResponseMessage response = await httpClient.PostAsync(licenseServerUri, httpRequestContent);
                response.EnsureSuccessStatusCode();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lastErrorMessage = string.Empty;
                    return response.Content;
                }
                else
                {
                    lastErrorMessage = "AcquireLicense - Http Response Status Code: " + response.StatusCode.ToString();
                }
            }
            catch (Exception exception)
            {
                lastErrorMessage = exception.Message;
                return null;
            }
            return null;
        }
    }
}
