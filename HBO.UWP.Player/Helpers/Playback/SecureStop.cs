//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Diagnostics;
using System.Linq;
using Windows.Media.Protection.PlayReady;

namespace HBO.UWP.Player.Helpers.Playback
{

    public class SecureStop : ServiceRequest
    {
        byte[] _SecureStopCert = null;

        public byte[] GetSecureStopCertificate()
        {
            return _SecureStopCert;
        }

        public void SetSecureStopCertificate(byte[] SecureStopCert)
        {
            _SecureStopCert = SecureStopCert;
        }

        protected virtual void SecureStopServiceRequestCompleted(PlayReadySecureStopServiceRequest sender, Exception hrCompletionStatus)
        {
            Debug.WriteLine("SecureStopServiceRequestCompleted");

            if (hrCompletionStatus != null)
            {
                Debug.WriteLine("SecureStopServiceRequestCompleted failed with " + hrCompletionStatus.HResult);
            }
        }

        void HandleIndivServiceRequest_Finished(bool bResult, object resultContext)
        {
            Debug.WriteLine("Enter SecureStop.HandleIndivServiceRequest_Finished()");

            Debug.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());

            if (bResult)
            {
                SecureStopProactively();
            }

            Debug.WriteLine("Leave SecureStop.HandleIndivServiceRequest_Finished()");
        }

        public void SecureStopProactively()
        {
            Debug.WriteLine("Enter SecureStop.SecureStopReportProactively()");
            try
            {
                Debug.WriteLine("Creating SecureStop report service request...");

                PlayReadySecureStopIterable secureStopIterable = new PlayReadySecureStopIterable(_SecureStopCert);

                PlayReadySecureStopServiceRequest SecureStopRequest = secureStopIterable.First() as PlayReadySecureStopServiceRequest;

                //PlayReadySecureStopServiceRequest SecureStopRequest = new PlayReadySecureStopServiceRequest(_SecureStopCert);
                SecureStopReactively(SecureStopRequest);
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
                    Debug.WriteLine("SecureStopProactively failed:" + ex.HResult);
                }
            }

            Debug.WriteLine("Leave SecureStop.SecureStopReportProactively()");
        }

        void ConfigureServiceRequest()
        {
            PlayReadySecureStopServiceRequest SecureStopRequest = _serviceRequest as PlayReadySecureStopServiceRequest;

            Debug.WriteLine(" ");
            Debug.WriteLine("Configure SecureStop request to these values:");
            if (RequestConfigData.Uri != null)
            {
                Debug.WriteLine("URL       :" + RequestConfigData.Uri.ToString());
                SecureStopRequest.Uri = RequestConfigData.Uri;
            }

            if (RequestConfigData.ChallengeCustomData != null && RequestConfigData.ChallengeCustomData != String.Empty)
            {
                Debug.WriteLine("ChallengeCustomData:" + RequestConfigData.ChallengeCustomData);
                SecureStopRequest.ChallengeCustomData = RequestConfigData.ChallengeCustomData;
            }

            Debug.WriteLine(" ");
        }

        async public void SecureStopReactively(PlayReadySecureStopServiceRequest SecureStopRequest)
        {
            Debug.WriteLine("Enter SecureStop.SecureStopReportReactively()");
            Exception exception = null;

            try
            {
                _serviceRequest = SecureStopRequest;
                ConfigureServiceRequest();

                Debug.WriteLine("ChallengeCustomData = " + SecureStopRequest.ChallengeCustomData);
                if (RequestConfigData.ManualEnabling)
                {
                    Debug.WriteLine("Manually posting the request...");

                    HttpHelper httpHelper = new HttpHelper(SecureStopRequest);
                    await httpHelper.GenerateChallengeAndProcessResponse();
                }
                else
                {
                    Debug.WriteLine("Begin SecureStop service request...");
                    await SecureStopRequest.BeginServiceRequest();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Saving exception..");
                exception = ex;
            }
            finally
            {
                Debug.WriteLine("Post-SecureStop Values:");
                if (exception == null)
                {
                    Debug.WriteLine("ResponseCustomData = " + SecureStopRequest.ResponseCustomData);
                    Debug.WriteLine("ProtectionSystem   = " + SecureStopRequest.ProtectionSystem.ToString());
                    Debug.WriteLine("Type = " + SecureStopRequest.Type.ToString());
                }

                SecureStopServiceRequestCompleted(SecureStopRequest, exception);
            }

            Debug.WriteLine("Leave SecureStop.SecureStopReportReactively()");
        }

    }

    public class SecureStopAndReportResult : SecureStop
    {
        ReportResultDelegate _reportResult = null;
        bool _bExpectError = false;
        
        public SecureStopAndReportResult(ReportResultDelegate callback, bool bExpectError)
        {
            _reportResult = callback;
            _bExpectError = bExpectError;
        }

        protected override void SecureStopServiceRequestCompleted(PlayReadySecureStopServiceRequest SecureStopRequest, Exception hrCompletionStatus)
        {
            Debug.WriteLine("Enter SecureStopAndReportResult.SecureStopServiceRequestCompleted()");

            if (hrCompletionStatus != null)
            {
                if (PerformEnablingActionIfRequested(hrCompletionStatus) || HandleExpectedError(hrCompletionStatus))
                {
                    Debug.WriteLine("Exception handled.");
                }
                else
                {
                    Debug.WriteLine("SecureStopServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                    _reportResult(false, null);
                }
            }

            Debug.WriteLine("Leave SecureStopAndReportResult.SecureStopServiceRequestCompleted()");
        }

        protected override void EnablingActionCompleted(bool bResult)
        {
            Debug.WriteLine("Enter SecureStopAndReportResult.EnablingActionCompleted()");

            _reportResult(bResult, null);

            Debug.WriteLine("Leave SecureStopAndReportResult.EnablingActionCompleted()");
        }

        protected override bool HandleExpectedError(Exception ex)
        {
            Debug.WriteLine("Enter SecureStopAndReportResult.HandleExpectedError() _bExpectError=" + _bExpectError);
            bool bHandled = false;

            if (_bExpectError)
            {
                Debug.WriteLine(" ex.HResult= " + ex.HResult);
                if (ex.HResult == DRM_E_NOMORE_DATA)
                {
                    Debug.WriteLine("SecureStopAndReportResult.HandleExpectedError : Received DRM_E_NOMORE_DATA as expected");
                    bHandled = true;
                    _reportResult(true, null);
                }
            }
                
            Debug.WriteLine("Leave SecureStopAndReportResult.HandleExpectedError()");
            return bHandled;
        }
    }
}
