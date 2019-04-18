using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Media;
using Windows.Foundation.Collections;
using Windows.Media.Protection;
using Windows.Media.Protection.PlayReady;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GO.UWP.Player.Helpers;
using GO.UWP.Player.Helpers.Playback;
using GO.UWP.Player.Model;
using GO.UWP.Player.ViewModel;
using Microsoft.PlayerFramework.Adaptive;
using PlayReadySample;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GO.UWP.Player.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DebugPlayerPage : Page
    {
        private MainViewModel mvm => (MainViewModel)DataContext;

        private PlayerArguments arguments;

        public DebugPlayerPage()
        {
            this.InitializeComponent();

            Player.MediaOpened += OnMediaOpened;
            Player.MediaFailed += OnMediaFailed;
        }

        public void SetupRequestConfigData(Guid customerId, Purchase purchase)
        {
            RequestConfigData = new ServiceRequestConfigData()
            {
                Uri = new Uri($"https://lic.drmtoday.com/license-proxy-headerauth/drmtoday/RightsManager.asmx?assetId={purchase.AssetId}&variantId={purchase.VariantId}"),
                ChallengeCustomData = Base64Helpers.Base64Encode("{\"userId\":\"" + customerId + "\",\"sessionId\":\"" + purchase.PlayerSessionId + "\",\"merchant\":\"hboeurope\"}"),
                CustomArtibutes = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("x-dt-auth-token", purchase.AuthToken),
                    new KeyValuePair<string, string>("Origin", "https://www.hbogo.cz"),
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetupRequestConfigData(mvm.CurrentUser.Customer.Id, mvm.CurrentlySelectedVideo.Purchase);
            var playUri = new Uri(mvm.CurrentlySelectedVideo.Purchase.MediaUrl.AbsoluteUri + "/manifest");

            arguments = new PlayerArguments()
            {
                RightsManagerUrl = RequestConfigData.Uri.ToString(),
                StreamUrl = playUri.ToString()
            };

            //arguments = e.Parameter as PlayerArguments;

            if (arguments != null)
            {
                InitializePlugins();
                InitializeMediaExtensionManager();
                InitializeMediaProtectionManager();

                Player.Source = new Uri(arguments.StreamUrl);
            }
        }

        /// <summary>Initializes the Smooth Streaming plugin.</summary>
        private void InitializePlugins()
        {
            var adaptivePlugin = new AdaptivePlugin(){InstreamCaptionsEnabled = true};

            Player.Plugins.Add(adaptivePlugin);
        }

        /// <summary>Initializes the media extension manager so we can handle PlayReady protected content.</summary>
        private void InitializeMediaExtensionManager()
        {
            var plugins = new MediaExtensionManager();

            // Add support for IIS Smooth Streaming Manifests
            plugins.RegisterByteStreamHandler("Microsoft.Media.AdaptiveStreaming.SmoothByteStreamHandler", ".ism", "text/xml");
            plugins.RegisterByteStreamHandler("Microsoft.Media.AdaptiveStreaming.SmoothByteStreamHandler", ".ism", "application/vnd.ms-sstr+xml");

            // Add support for PlayReady video and audio files
            plugins.RegisterByteStreamHandler("Microsoft.Media.Protection.PlayReady.PlayReadyByteStreamHandler", ".pyv", "");
            plugins.RegisterByteStreamHandler("Microsoft.Media.Protection.PlayReady.PlayReadyByteStreamHandler", ".pya", "");
        }

        /// <summary>Initializes the PlayReady protection manager.</summary>
        private void InitializeMediaProtectionManager()
        {
            var mediaProtectionManager = new MediaProtectionManager();
            mediaProtectionManager.ComponentLoadFailed += OnMediaProtectionManagerComponentLoadFailed;
            mediaProtectionManager.ServiceRequested += OnMediaProtectionManagerServiceRequested;

            // Set up the container GUID for the CFF format (used with DASH streams), see http://uvdemystified.com/uvfaq.html#3.2
            // The GUID represents MPEG DASH Content Protection using Microsoft PlayReady, see http://dashif.org/identifiers/protection/
            mediaProtectionManager.Properties["Windows.Media.Protection.MediaProtectionContainerGuid"] = "{9A04F079-9840-4286-AB92-E65BE0885F95}";

            // Set up the drm layer to use. Hardware DRM is the default, but not all older hardware supports this
            var supportsHardwareDrm = PlayReadyStatics.CheckSupportedHardware(PlayReadyHardwareDRMFeatures.HardwareDRM);
            if (!supportsHardwareDrm)
            {
                mediaProtectionManager.Properties["Windows.Media.Protection.UseSoftwareProtectionLayer"] = true;
            }

            // Set up the content protection manager so it uses the PlayReady Input Trust Authority (ITA) for the relevant media sources
            // The MediaProtectionSystemId GUID is format and case sensitive, see https://msdn.microsoft.com/en-us/library/windows.media.protection.mediaprotectionmanager.properties.aspx
            var cpsystems = new PropertySet();
            cpsystems[PlayReadyStatics.MediaProtectionSystemId.ToString("B").ToUpper()] = "Windows.Media.Protection.PlayReady.PlayReadyWinRTTrustedInput";
            mediaProtectionManager.Properties["Windows.Media.Protection.MediaProtectionSystemIdMapping"] = cpsystems;
            mediaProtectionManager.Properties["Windows.Media.Protection.MediaProtectionSystemId"] = PlayReadyStatics.MediaProtectionSystemId.ToString("B").ToUpper();

            Player.ProtectionManager = mediaProtectionManager;
        }

        private void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            // Start playing the file when ready
            Player.Play();
        }

        private void OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("Media Failed: " + e.ErrorMessage);
        }

        private void OnMediaProtectionManagerComponentLoadFailed(MediaProtectionManager sender, ComponentLoadFailedEventArgs e)
        {
            Debug.WriteLine("ProtectionManager ComponentLoadFailed");
            e.Completion.Complete(false);
        }

        public ServiceRequestConfigData RequestConfigData
        {
            set { this._requestConfigData = value; }
            get { return this._requestConfigData; }
        }

        RequestChain _requestChain = null;
        ServiceRequestConfigData _requestConfigData = null;
        MediaProtectionServiceCompletion _serviceCompletionNotifier = null;

        private async void OnMediaProtectionManagerServiceRequested(MediaProtectionManager sender, ServiceRequestedEventArgs e)
        {
            Debug.WriteLine("ProtectionManager ServiceRequested");

            var completionNotifier = e.Completion;
            var request = (IPlayReadyServiceRequest)e.Request;


            _requestChain = new RequestChain(request);
            _requestChain.RequestConfigData = this.RequestConfigData;
            _requestChain.FinishAndReportResult(new ReportResultDelegate(HandleServiceRequest_Finished));

            var result = false;
            
            if (request.Type == PlayReadyStatics.IndividualizationServiceRequestType)
            {
                result = await PlayReadyLicenseHandler.RequestIndividualizationToken(request as PlayReadyIndividualizationServiceRequest);
            }
            else if (request.Type == PlayReadyStatics.LicenseAcquirerServiceRequestType)
            {
                // NOTE: You might need to set the request.ChallengeCustomData, depending on your Rights Manager.
                if (!string.IsNullOrEmpty(arguments.RightsManagerUrl))
                {
                    request.Uri = new Uri(arguments.RightsManagerUrl);
                }

                result = await PlayReadyLicenseHandler.RequestLicense(request as PlayReadyLicenseAcquisitionServiceRequest);
            }

            completionNotifier.Complete(result);
        }

        void HandleServiceRequest_Finished(bool bResult, object resultContext)
        {
            Debug.WriteLine("Enter Playback.HandleServiceRequest_Finished()");

            Debug.WriteLine("MediaProtectionServiceCompletion.Complete = " + bResult.ToString());
            _serviceCompletionNotifier.Complete(bResult);

            Debug.WriteLine("Leave Playback.HandleServiceRequest_Finished()");
        }

        private void ButtonPlayPause_OnClick(object sender, RoutedEventArgs e)
        {
            if (Player.CurrentState != MediaElementState.Paused) Player.Pause();
            else Player.PlayResume();
        }

        private void ButtonAudioStream_OnClick(object sender, RoutedEventArgs e)
        {
            if (Player.AvailableAudioStreams.Count > 1) Player.SelectedAudioStream = Player.AvailableAudioStreams.First(x => x != Player.SelectedAudioStream);
        }

        private void ButtonSubtitles_OnClick(object sender, RoutedEventArgs e)
        {
            Player.IsCaptionsActive = true;
            if (Player.SelectedCaption == null && Player.AvailableCaptions.Count >= 1) Player.SelectedCaption = Player.AvailableCaptions.First();
            else if (Player.AvailableCaptions.Count > 1) Player.SelectedCaption = Player.AvailableCaptions.First(x => x != Player.SelectedCaption);
        }
    }

    public class PlayerArguments
    {
        public string StreamUrl { get; set; }

        public string RightsManagerUrl { get; set; }
    }
}

namespace PlayReadySample
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Windows.Media.Protection.PlayReady;

    public class PlayReadyLicenseHandler
    {
        /// <summary>Request a token that identifies the player session.</summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if successfull, <c>false</c> otherwise.</returns>
        public static async Task<bool> RequestIndividualizationToken(PlayReadyIndividualizationServiceRequest request)
        {
            Debug.WriteLine("ProtectionManager PlayReady Individualization Service Request in progress");

            try
            {
                Debug.WriteLine("Requesting individualization token from {0}", request.Uri);

                await request.BeginServiceRequest();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ProtectionManager PlayReady Individualization Service Request failed: " + ex.Message);

                return false;
            }

            Debug.WriteLine("ProtectionManager PlayReady Individualization Service Request successfull");

            return true;
        }

        /// <summary>Request a license for playing a stream.</summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if successfull, <c>false</c> otherwise.</returns>
        public static async Task<bool> RequestLicense(PlayReadyLicenseAcquisitionServiceRequest request)
        {
            Debug.WriteLine("ProtectionManager PlayReady License Request in progress");

            try
            {
                Debug.WriteLine("Requesting license from {0} with custom data {1}", request.Uri, request.ChallengeCustomData);

                await request.BeginServiceRequest();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ProtectionManager PlayReady License Request failed: " + ex.Message);

                return false;
            }

            Debug.WriteLine("ProtectionManager PlayReady License Request successfull");

            return true;
        }

        public static async Task<bool> RequestLicenseManual(PlayReadyLicenseAcquisitionServiceRequest request)
        {
            Debug.WriteLine("ProtectionManager PlayReady Manual License Request in progress");

            try
            {
                var r = request.GenerateManualEnablingChallenge();

                var content = new ByteArrayContent(r.GetMessageBody());

                foreach (var header in r.MessageHeaders.Where(x => x.Value != null))
                {
                    if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        content.Headers.ContentType = MediaTypeHeaderValue.Parse(header.Value.ToString());
                    }
                    else
                    {
                        content.Headers.Add(header.Key, header.Value.ToString());
                    }
                }

                var msg = new HttpRequestMessage(HttpMethod.Post, r.Uri) { Content = content };

                Debug.WriteLine("Requesting license from {0} with custom data {1}", msg.RequestUri, await msg.Content.ReadAsStringAsync());

                var client = new HttpClient();
                var response = await client.SendAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    request.ProcessManualEnablingResponse(await response.Content.ReadAsByteArrayAsync());
                }
                else
                {
                    Debug.WriteLine("ProtectionManager PlayReady License Request failed: " + await response.Content.ReadAsStringAsync());

                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ProtectionManager PlayReady License Request failed: " + ex.Message);

                return false;
            }

            Debug.WriteLine("ProtectionManager PlayReady License Request successfull");

            return true;
        }
    }
}
