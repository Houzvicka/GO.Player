//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Protection;
using Windows.Media.Protection.PlayReady;
using HBO.UWP.Player.Model;
using PlayReadyUAP;

namespace PlayReadyUAP
{
    public class LicenseAcquisition : ServiceRequest
    {
        private bool bPersistent = false;
        private PlayReadyLicenseSession licenseSession;
        private static List<KeyValuePair<string, string>> customHeaders;

        public bool Persistent
        {
            set { this.bPersistent = value; }
            get { return this.bPersistent; }
        }

        protected virtual void LAServiceRequestCompleted( IPlayReadyLicenseAcquisitionServiceRequest  sender, Exception hrCompletionStatus )
        {
        }

        void HandleIndivServiceRequest_Finished(bool bResult,object resultContext)
        {
            Console.WriteLine("Enter LicenseAcquisition.HandleIndivServiceRequest_Finished()");

            Console.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());
            if (bResult)
            {
                AcquireLicenseProactively();
            }

            Console.WriteLine("Leave LicenseAcquisition.HandleIndivServiceRequest_Finished()");
        }

        public PlayReadyLicenseSession createLicenseSession ()
        {
            Console.WriteLine("Enter createLicenseSession");

            //A setting to tell MF that we are using PlayReady.
           var propSet = new Windows.Foundation.Collections.PropertySet();
            propSet["Windows.Media.Protection.MediaProtectionSystemId"] = "{F4637010-03C3-42CD-B932-B48ADF3A6A54}";

            var cpsystems = new Windows.Foundation.Collections.PropertySet();
            cpsystems["{F4637010-03C3-42CD-B932-B48ADF3A6A54}"] = "Windows.Media.Protection.PlayReady.PlayReadyWinRTTrustedInput"; //Playready TrustedInput Class Name
            propSet["Windows.Media.Protection.MediaProtectionSystemIdMapping"] = cpsystems;

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Containers.ContainsKey("PlayReady") &&
                localSettings.Containers["PlayReady"].Values.ContainsKey("SoftwareOverride"))
            {
                int UseSoftwareProtectionLayer = (int)localSettings.Containers["PlayReady"].Values["SoftwareOverride"];

                if (UseSoftwareProtectionLayer == 1)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("***** Use Software Protection Layer for createLicenseSession ******");
                    propSet["Windows.Media.Protection.UseSoftwareProtectionLayer"] = true;
                }
            }

            //Create the MF media session that the license will be tied to
            var pmpServer = new Windows.Media.Protection.MediaProtectionPMPServer(propSet);

            var propSet2 = new Windows.Foundation.Collections.PropertySet();
            //Set the property for the LicenseSession. This tells PlayReady to tie the license to that particular media session
            propSet2["Windows.Media.Protection.MediaProtectionPMPServer"] = pmpServer;

            this.licenseSession = new Windows.Media.Protection.PlayReady.PlayReadyLicenseSession(propSet2);
            Console.WriteLine("Exit createLicenseSession");
            return this.licenseSession;
        }
        public void configMediaProtectionManager(MediaProtectionManager mediaProtectionManager)
        {
            //This handles the proactive LA of in memory license. 
            if (this.licenseSession == null)
            {
                createLicenseSession();
            }

            //LicenseSession will set the proper setting in the media protection manager so that
            //MF knows which existing media session to use and to use PlayReady as the DRM system
            this.licenseSession.ConfigureMediaProtectionManager(mediaProtectionManager);
        }

        public void  AcquireLicenseProactively()
        {
            IPlayReadyLicenseAcquisitionServiceRequest licenseRequest;

            try
            {
                PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(  0,
                                                                                    RequestConfigData.KeyIds,
                                                                                    null,
                                                                                    RequestConfigData.EncryptionAlgorithm,
                                                                                    null,
                                                                                    null,
                                                                                    String.Empty,
                                                                                    RequestConfigData.DomainServiceId);

                Console.WriteLine("Creating license acquisition service request...");
                
                if(bPersistent)
                {   // persistent license
                    licenseRequest = (IPlayReadyLicenseAcquisitionServiceRequest) new PlayReadyLicenseAcquisitionServiceRequest();
                }
                else
                {
                    if (this.licenseSession == null)
                    {
                        throw new ArgumentNullException("licenseSession can not be null");
                    }

                    //in-memory license, use license session to create a license service request
                    //this way, the acquired license will be tied to the media session associated with the license session
                    licenseRequest = this.licenseSession.CreateLAServiceRequest();
                }
                licenseRequest.ContentHeader = contentHeader;
                licenseRequest.Uri = RequestConfigData.Uri;
                AcquireLicenseReactively(licenseRequest);
            }
            catch (Exception ex)
            {
                if (ex.HResult == ServiceRequest.MSPR_E_NEEDS_INDIVIDUALIZATION)
                {
                    PlayReadyIndividualizationServiceRequest indivServiceRequest = new PlayReadyIndividualizationServiceRequest();

                    RequestChain requestChain = new RequestChain(indivServiceRequest);
                    requestChain.FinishAndReportResult(new ReportResultDelegate(HandleIndivServiceRequest_Finished));
                }
                else
                {
                    //Console.WriteLine("AcquireLicenseProactively failed:" + ex.HResult);
                    Console.WriteLine("AcquireLicenseProactively failed:" + ex.HResult);
                    licenseRequest = (IPlayReadyLicenseAcquisitionServiceRequest)new PlayReadyLicenseAcquisitionServiceRequest();
                    LAServiceRequestCompleted(licenseRequest, ex);
                }
            }
            
        }

        static public void DumpContentHeaderValues(PlayReadyContentHeader contentHeader)
        {
            Console.WriteLine(" " );
            Console.WriteLine("Content header values:" );
            if( contentHeader == null )
            {
                return;
            }
            Console.WriteLine("CustomAttributes :" + contentHeader.CustomAttributes);
            Console.WriteLine("DecryptorSetup   :" + contentHeader.DecryptorSetup.ToString());
            Console.WriteLine("DomainServiceId  :" + contentHeader.DomainServiceId.ToString());
            Console.WriteLine("EncryptionType   :" + contentHeader.EncryptionType.ToString());
            for (int i = 0; i < contentHeader.KeyIds.Length; i++)
            {
                Console.WriteLine("KeyId " + i + "       :" + contentHeader.KeyIds[i].ToString());
                Console.WriteLine("KeyIdString " + i + " :" + contentHeader.KeyIdStrings[i]);
            }
            
            if (contentHeader.LicenseAcquisitionUrl != null)
            {
                Console.WriteLine("LicenseAcquisitionUrl :" + contentHeader.LicenseAcquisitionUrl.ToString());
            }          
        }

        void ConfigureServiceRequest()
        {
            PlayReadyLicenseAcquisitionServiceRequest licenseRequest = _serviceRequest as PlayReadyLicenseAcquisitionServiceRequest;
                        
            Console.WriteLine(" " );
            Console.WriteLine("Configure license request to these values:" );

            if (RequestConfigData.Uri != null)
            {
                Console.WriteLine("LA URL       :" + RequestConfigData.Uri.ToString());
                licenseRequest.Uri = RequestConfigData.Uri;
            }

            if (RequestConfigData.CustomArtibutes != null && RequestConfigData.CustomArtibutes.Count >= 1)
            {
                customHeaders = RequestConfigData.CustomArtibutes;
            }

            if (RequestConfigData.ChallengeCustomData != null && RequestConfigData.ChallengeCustomData != String.Empty)
            {
                Console.WriteLine("ChallengeCustomData:" + RequestConfigData.ChallengeCustomData);
                licenseRequest.ChallengeCustomData = RequestConfigData.ChallengeCustomData;
            }

            Console.WriteLine(" " );
        }
        
        async public void AcquireLicenseReactively(IPlayReadyLicenseAcquisitionServiceRequest licenseRequest)
        {
            Console.WriteLine("Enter LicenseAcquisition.AcquireLicenseReactively()" );
            Exception exception = null;
            
            try
            {   
                _serviceRequest = licenseRequest;
                ConfigureServiceRequest();
                SerivceRequestStatistics.IncLicenseAcquisitionCount();

                Console.WriteLine("ChallengeCustomData = " + licenseRequest.ChallengeCustomData);
                if( RequestConfigData.ManualEnabling )
                {
                    Console.WriteLine("Manually posting the request..." );
                    
                    HttpHelper httpHelper = new HttpHelper( licenseRequest );
                    await httpHelper.GenerateChallengeAndProcessResponse();
                }
                else
                {
                    Console.WriteLine("Begin license acquisition service request..." );
                    await licenseRequest.BeginServiceRequest();
                }

                Console.WriteLine("Post-LicenseAcquisition Values:");
                Console.WriteLine("DomainServiceId          = " + licenseRequest.DomainServiceId.ToString());
                DumpContentHeaderValues(licenseRequest.ContentHeader);
            }
            catch( Exception ex )
            {
                Console.WriteLine("Saving exception.. " + ex.ToString() );

                exception = ex;
            }
            finally
            {
                if( exception == null )
                {
                    Console.WriteLine("ResponseCustomData       = " + licenseRequest.ResponseCustomData);
                }               
                LAServiceRequestCompleted( licenseRequest, exception );
            }
            
            Console.WriteLine("Leave LicenseAcquisition.AcquireLicenseReactively()" );
        }

        public async void RequestLicenseManual(PlayReadyLicenseAcquisitionServiceRequest request)
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

                if (customHeaders != null && customHeaders.Count >= 1)
                {
                    foreach (KeyValuePair<string, string> header in customHeaders)
                    {
                        content.Headers.Add(header.Key, header.Value);
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ProtectionManager PlayReady License Request failed: " + ex.Message);
            }

            Debug.WriteLine("ProtectionManager PlayReady License Request successfull");
        }

    }

    public class LAAndReportResult : LicenseAcquisition
    {
        ReportResultDelegate _reportResult = null;
        string _strExpectedError = null;
        Purchase sampleDataItem = null;
        
        public string ExpectedError  
        {  
            set { this._strExpectedError =  value; }  
            get { return this._strExpectedError; } 
        }
        
        public LAAndReportResult( ReportResultDelegate callback)
        {
            _reportResult = callback;
        }

        public LAAndReportResult(ReportResultDelegate callback, Purchase item )
        {
            sampleDataItem = item;
            _reportResult = callback;
        }

        protected override void LAServiceRequestCompleted( IPlayReadyLicenseAcquisitionServiceRequest  sender, Exception hrCompletionStatus )
        {
            Console.WriteLine("Enter LAAndReportResult.LAServiceRequestCompleted()" );

            if( hrCompletionStatus == null )
            {
                Console.WriteLine("***License acquisition succeeded***");
               _reportResult( true, sampleDataItem);
            }
            else
            {
                if (PerformEnablingActionIfRequested(hrCompletionStatus) || HandleExpectedError(hrCompletionStatus))
                {
                    Console.WriteLine("Exception handled.");
                }
                else
                {
                    Console.WriteLine("LAServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                    Console.WriteLine("hrCompletionStatus.HResult=" +  hrCompletionStatus.HResult.ToString());
                    _reportResult(false, sampleDataItem);
                }
            }
                
            Console.WriteLine("Leave LAAndReportResult.LAServiceRequestCompleted()" );
        }
        
        protected override void EnablingActionCompleted(bool bResult)
        {
            Console.WriteLine("Enter LAAndReportResult.EnablingActionCompleted()" );

            _reportResult( bResult, sampleDataItem);
            
            Console.WriteLine("Leave LAAndReportResult.EnablingActionCompleted()" );
        }

        protected override bool HandleExpectedError(Exception ex)
        {
            Console.WriteLine("Enter LAAndReportResult.HandleExpectedError()" );
            
            if( string.IsNullOrEmpty( _strExpectedError ) )
            {
                Console.WriteLine("Setting error code to " + RequestConfigData.ExpectedLAErrorCode );
                _strExpectedError = RequestConfigData.ExpectedLAErrorCode;
            }
            
            bool bHandled = false;
            if( !string.IsNullOrEmpty(_strExpectedError) )
            {
                if ( ex.Message.ToLower().Contains( _strExpectedError.ToLower() ) )
                {
                    Console.WriteLine( "'" + ex.Message + "' Contains " + _strExpectedError + "  as expected" );
                    bHandled = true;
                    _reportResult( true, sampleDataItem );
                }
            }
            
            Console.WriteLine("Leave LAAndReportResult.HandleExpectedError()" );
            return bHandled;
        }
        
    }

}
