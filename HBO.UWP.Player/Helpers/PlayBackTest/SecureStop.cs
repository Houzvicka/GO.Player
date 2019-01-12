//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Media.Protection.PlayReady;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using PlayReadyUAP;

namespace PlayReadyUAP
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
            Console.WriteLine("SecureStopServiceRequestCompleted");

            if (hrCompletionStatus != null)
            {
                Console.WriteLine("SecureStopServiceRequestCompleted failed with " + hrCompletionStatus.HResult);
            }
        }

        void HandleIndivServiceRequest_Finished(bool bResult, object resultContext)
        {
            Console.WriteLine("Enter SecureStop.HandleIndivServiceRequest_Finished()");

            Console.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());

            if (bResult)
            {
                SecureStopProactively();
            }

            Console.WriteLine("Leave SecureStop.HandleIndivServiceRequest_Finished()");
        }

        public void SecureStopProactively()
        {
            Console.WriteLine("Enter SecureStop.SecureStopReportProactively()");
            try
            {
                Console.WriteLine("Creating SecureStop report service request...");

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
                    Console.WriteLine("SecureStopProactively failed:" + ex.HResult);
                }
            }

            Console.WriteLine("Leave SecureStop.SecureStopReportProactively()");
        }

        void ConfigureServiceRequest()
        {
            PlayReadySecureStopServiceRequest SecureStopRequest = _serviceRequest as PlayReadySecureStopServiceRequest;

            Console.WriteLine(" ");
            Console.WriteLine("Configure SecureStop request to these values:");
            if (RequestConfigData.Uri != null)
            {
                Console.WriteLine("URL       :" + RequestConfigData.Uri.ToString());
                SecureStopRequest.Uri = RequestConfigData.Uri;
            }

            if (RequestConfigData.ChallengeCustomData != null && RequestConfigData.ChallengeCustomData != String.Empty)
            {
                Console.WriteLine("ChallengeCustomData:" + RequestConfigData.ChallengeCustomData);
                SecureStopRequest.ChallengeCustomData = RequestConfigData.ChallengeCustomData;
            }

            Console.WriteLine(" ");
        }

        async public void SecureStopReactively(PlayReadySecureStopServiceRequest SecureStopRequest)
        {
            Console.WriteLine("Enter SecureStop.SecureStopReportReactively()");
            Exception exception = null;

            try
            {
                _serviceRequest = SecureStopRequest;
                ConfigureServiceRequest();

                Console.WriteLine("ChallengeCustomData = " + SecureStopRequest.ChallengeCustomData);
                if (RequestConfigData.ManualEnabling)
                {
                    Console.WriteLine("Manually posting the request...");

                    HttpHelper httpHelper = new HttpHelper(SecureStopRequest);
                    await httpHelper.GenerateChallengeAndProcessResponse();
                }
                else
                {
                    Console.WriteLine("Begin SecureStop service request...");
                    await SecureStopRequest.BeginServiceRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Saving exception..");
                exception = ex;
            }
            finally
            {
                Console.WriteLine("Post-SecureStop Values:");
                if (exception == null)
                {
                    Console.WriteLine("ResponseCustomData = " + SecureStopRequest.ResponseCustomData);
                    Console.WriteLine("ProtectionSystem   = " + SecureStopRequest.ProtectionSystem.ToString());
                    Console.WriteLine("Type = " + SecureStopRequest.Type.ToString());
                }

                SecureStopServiceRequestCompleted(SecureStopRequest, exception);
            }

            Console.WriteLine("Leave SecureStop.SecureStopReportReactively()");
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
            Console.WriteLine("Enter SecureStopAndReportResult.SecureStopServiceRequestCompleted()");

            if (hrCompletionStatus != null)
            {
                if (PerformEnablingActionIfRequested(hrCompletionStatus) || HandleExpectedError(hrCompletionStatus))
                {
                    Console.WriteLine("Exception handled.");
                }
                else
                {
                    Console.WriteLine("SecureStopServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                    _reportResult(false, null);
                }
            }

            Console.WriteLine("Leave SecureStopAndReportResult.SecureStopServiceRequestCompleted()");
        }

        protected override void EnablingActionCompleted(bool bResult)
        {
            Console.WriteLine("Enter SecureStopAndReportResult.EnablingActionCompleted()");

            _reportResult(bResult, null);

            Console.WriteLine("Leave SecureStopAndReportResult.EnablingActionCompleted()");
        }

        protected override bool HandleExpectedError(Exception ex)
        {
            Console.WriteLine("Enter SecureStopAndReportResult.HandleExpectedError() _bExpectError=" + _bExpectError);
            bool bHandled = false;

            if (_bExpectError)
            {
                Console.WriteLine(" ex.HResult= " + ex.HResult);
                if (ex.HResult == DRM_E_NOMORE_DATA)
                {
                    Console.WriteLine("SecureStopAndReportResult.HandleExpectedError : Received DRM_E_NOMORE_DATA as expected");
                    bHandled = true;
                    _reportResult(true, null);
                }
            }
                
            Console.WriteLine("Leave SecureStopAndReportResult.HandleExpectedError()");
            return bHandled;
        }
    }
}
