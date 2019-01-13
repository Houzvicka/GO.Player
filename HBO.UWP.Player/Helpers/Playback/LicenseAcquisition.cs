//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Media.Protection;
using Windows.Media.Protection.PlayReady;
using HBO.UWP.Player.Model;

namespace HBO.UWP.Player.Helpers.Playback
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

        protected virtual void LAServiceRequestCompleted(IPlayReadyLicenseAcquisitionServiceRequest sender,
            Exception hrCompletionStatus)
        {
        }

        void HandleIndivServiceRequest_Finished(bool bResult, object resultContext)
        {
            Debug.WriteLine("Enter LicenseAcquisition.HandleIndivServiceRequest_Finished()");

            Debug.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());
            if (bResult)
            {
                AcquireLicenseProactively();
            }

            Debug.WriteLine("Leave LicenseAcquisition.HandleIndivServiceRequest_Finished()");
        }

        public PlayReadyLicenseSession createLicenseSession()
        {
            Debug.WriteLine("Enter createLicenseSession");

            //A setting to tell MF that we are using PlayReady.
            var propSet = new Windows.Foundation.Collections.PropertySet();
            propSet["Windows.Media.Protection.MediaProtectionSystemId"] = "{F4637010-03C3-42CD-B932-B48ADF3A6A54}";

            var cpsystems = new Windows.Foundation.Collections.PropertySet();
            cpsystems["{F4637010-03C3-42CD-B932-B48ADF3A6A54}"] =
                "Windows.Media.Protection.PlayReady.PlayReadyWinRTTrustedInput"; //Playready TrustedInput Class Name
            propSet["Windows.Media.Protection.MediaProtectionSystemIdMapping"] = cpsystems;

            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Containers.ContainsKey("PlayReady") &&
                localSettings.Containers["PlayReady"].Values.ContainsKey("SoftwareOverride"))
            {
                int UseSoftwareProtectionLayer = (int) localSettings.Containers["PlayReady"].Values["SoftwareOverride"];

                if (UseSoftwareProtectionLayer == 1)
                {
                    Debug.WriteLine(" ");
                    Debug.WriteLine("***** Use Software Protection Layer for createLicenseSession ******");
                    propSet["Windows.Media.Protection.UseSoftwareProtectionLayer"] = true;
                }
            }

            //Create the MF media session that the license will be tied to
            var pmpServer = new Windows.Media.Protection.MediaProtectionPMPServer(propSet);

            var propSet2 = new Windows.Foundation.Collections.PropertySet();
            //Set the property for the LicenseSession. This tells PlayReady to tie the license to that particular media session
            propSet2["Windows.Media.Protection.MediaProtectionPMPServer"] = pmpServer;

            this.licenseSession = new Windows.Media.Protection.PlayReady.PlayReadyLicenseSession(propSet2);
            Debug.WriteLine("Exit createLicenseSession");
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

        public void AcquireLicenseProactively()
        {
            IPlayReadyLicenseAcquisitionServiceRequest licenseRequest;

            try
            {
                PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(0,
                    RequestConfigData.KeyIds,
                    null,
                    RequestConfigData.EncryptionAlgorithm,
                    null,
                    null,
                    String.Empty,
                    RequestConfigData.DomainServiceId);

                Debug.WriteLine("Creating license acquisition service request...");

                if (bPersistent)
                {
                    // persistent license
                    licenseRequest =
                        (IPlayReadyLicenseAcquisitionServiceRequest) new PlayReadyLicenseAcquisitionServiceRequest();
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
                    PlayReadyIndividualizationServiceRequest indivServiceRequest =
                        new PlayReadyIndividualizationServiceRequest();

                    RequestChain requestChain = new RequestChain(indivServiceRequest);
                    requestChain.FinishAndReportResult(new ReportResultDelegate(HandleIndivServiceRequest_Finished));
                }
                else
                {
                    //Debug.WriteLine("AcquireLicenseProactively failed:" + ex.HResult);
                    Debug.WriteLine("AcquireLicenseProactively failed:" + ex.HResult);
                    licenseRequest =
                        (IPlayReadyLicenseAcquisitionServiceRequest) new PlayReadyLicenseAcquisitionServiceRequest();
                    LAServiceRequestCompleted(licenseRequest, ex);
                }
            }

        }

        static public void DumpContentHeaderValues(PlayReadyContentHeader contentHeader)
        {
            Debug.WriteLine(" ");
            Debug.WriteLine("Content header values:");
            if (contentHeader == null)
            {
                return;
            }

            Debug.WriteLine("CustomAttributes :" + contentHeader.CustomAttributes);
            Debug.WriteLine("DecryptorSetup   :" + contentHeader.DecryptorSetup.ToString());
            Debug.WriteLine("DomainServiceId  :" + contentHeader.DomainServiceId.ToString());
            Debug.WriteLine("EncryptionType   :" + contentHeader.EncryptionType.ToString());
            for (int i = 0; i < contentHeader.KeyIds.Length; i++)
            {
                Debug.WriteLine("KeyId " + i + "       :" + contentHeader.KeyIds[i].ToString());
                Debug.WriteLine("KeyIdString " + i + " :" + contentHeader.KeyIdStrings[i]);
            }

            if (contentHeader.LicenseAcquisitionUrl != null)
            {
                Debug.WriteLine("LicenseAcquisitionUrl :" + contentHeader.LicenseAcquisitionUrl.ToString());
            }
        }

        void ConfigureServiceRequest()
        {
            PlayReadyLicenseAcquisitionServiceRequest licenseRequest =
                _serviceRequest as PlayReadyLicenseAcquisitionServiceRequest;

            Debug.WriteLine(" ");
            Debug.WriteLine("Configure license request to these values:");

            if (RequestConfigData.Uri != null)
            {
                Debug.WriteLine("LA URL       :" + RequestConfigData.Uri.ToString());
                licenseRequest.Uri = RequestConfigData.Uri;
            }

            if (RequestConfigData.CustomArtibutes != null && RequestConfigData.CustomArtibutes.Count >= 1)
            {
                customHeaders = RequestConfigData.CustomArtibutes;
                RequestConfigData.ManualEnabling = true;
            }

            if (RequestConfigData.ChallengeCustomData != null && RequestConfigData.ChallengeCustomData != String.Empty)
            {
                Debug.WriteLine("ChallengeCustomData:" + RequestConfigData.ChallengeCustomData);
                licenseRequest.ChallengeCustomData = RequestConfigData.ChallengeCustomData;
            }

            Debug.WriteLine(" ");
        }

        async public void AcquireLicenseReactively(IPlayReadyLicenseAcquisitionServiceRequest licenseRequest)
        {
            Debug.WriteLine("Enter LicenseAcquisition.AcquireLicenseReactively()");
            Exception exception = null;

            try
            {
                _serviceRequest = licenseRequest;
                ConfigureServiceRequest();
                SerivceRequestStatistics.IncLicenseAcquisitionCount();

                Debug.WriteLine("ChallengeCustomData = " + licenseRequest.ChallengeCustomData);
                if (RequestConfigData.ManualEnabling)
                {
                    Debug.WriteLine("Manually posting the request...");

                    HttpHelper httpHelper = new HttpHelper(licenseRequest, customHeaders);
                    await httpHelper.GenerateChallengeAndProcessResponse();
                }
                else
                {
                    Debug.WriteLine("Begin license acquisition service request...");
                    await licenseRequest.BeginServiceRequest();
                }

                Debug.WriteLine("Post-LicenseAcquisition Values:");
                Debug.WriteLine("DomainServiceId          = " + licenseRequest.DomainServiceId.ToString());
                DumpContentHeaderValues(licenseRequest.ContentHeader);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Saving exception.. " + ex.ToString());

                exception = ex;
            }
            finally
            {
                if (exception == null)
                {
                    Debug.WriteLine("ResponseCustomData       = " + licenseRequest.ResponseCustomData);
                }

                LAServiceRequestCompleted(licenseRequest, exception);
            }

            Debug.WriteLine("Leave LicenseAcquisition.AcquireLicenseReactively()");
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
            Debug.WriteLine("Enter LAAndReportResult.LAServiceRequestCompleted()" );

            if( hrCompletionStatus == null )
            {
                Debug.WriteLine("***License acquisition succeeded***");
               _reportResult( true, sampleDataItem);
            }
            else
            {
                if (PerformEnablingActionIfRequested(hrCompletionStatus) || HandleExpectedError(hrCompletionStatus))
                {
                    Debug.WriteLine("Exception handled.");
                }
                else
                {
                    Debug.WriteLine("LAServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                    Debug.WriteLine("hrCompletionStatus.HResult=" +  hrCompletionStatus.HResult.ToString());
                    _reportResult(false, sampleDataItem);
                }
            }
                
            Debug.WriteLine("Leave LAAndReportResult.LAServiceRequestCompleted()" );
        }
        
        protected override void EnablingActionCompleted(bool bResult)
        {
            Debug.WriteLine("Enter LAAndReportResult.EnablingActionCompleted()" );

            _reportResult( bResult, sampleDataItem);
            
            Debug.WriteLine("Leave LAAndReportResult.EnablingActionCompleted()" );
        }

        protected override bool HandleExpectedError(Exception ex)
        {
            Debug.WriteLine("Enter LAAndReportResult.HandleExpectedError()" );
            
            if( string.IsNullOrEmpty( _strExpectedError ) )
            {
                Debug.WriteLine("Setting error code to " + RequestConfigData.ExpectedLAErrorCode );
                _strExpectedError = RequestConfigData.ExpectedLAErrorCode;
            }
            
            bool bHandled = false;
            if( !string.IsNullOrEmpty(_strExpectedError) )
            {
                if ( ex.Message.ToLower().Contains( _strExpectedError.ToLower() ) )
                {
                    Debug.WriteLine( "'" + ex.Message + "' Contains " + _strExpectedError + "  as expected" );
                    bHandled = true;
                    _reportResult( true, sampleDataItem );
                }
            }
            
            Debug.WriteLine("Leave LAAndReportResult.HandleExpectedError()" );
            return bHandled;
        }
        
    }

}
