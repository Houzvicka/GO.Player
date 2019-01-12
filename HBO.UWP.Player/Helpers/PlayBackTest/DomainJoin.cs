//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using Windows.Foundation;
using Windows.Media.Protection.PlayReady;
using PlayReadyUAP;

namespace PlayReadyUAP
{
    public class DomainJoin : ServiceRequest
    {

        protected virtual void DomainJoinServiceRequestCompleted(PlayReadyDomainJoinServiceRequest sender, Exception hrCompletionStatus)
        {
            Console.WriteLine("DomainJoinServiceRequestCompleted");

            if (hrCompletionStatus != null)
            {
                Console.WriteLine("DomainJoinServiceRequestCompleted failed with " + hrCompletionStatus.HResult);
            }
        }

        void HandleIndivServiceRequest_Finished(bool bResult, object resultContext)
        {
            Console.WriteLine("Enter DomainJoin.HandleIndivServiceRequest_Finished()");

            Console.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());
            if (bResult)
            {
                DomainJoinProactively();
            }

            Console.WriteLine("Leave DomainJoin.HandleIndivServiceRequest_Finished()");
        }

        public void DomainJoinProactively()
        {
            Console.WriteLine("Enter DomainJoin.DomainJoinProactively()");
            try
            {
                PlayReadyDomainJoinServiceRequest domainJoinRequest = new PlayReadyDomainJoinServiceRequest();
                domainJoinRequest.DomainServiceId = RequestConfigData.DomainServiceId;
                domainJoinRequest.DomainAccountId = RequestConfigData.DomainAccountId;
                domainJoinRequest.Uri = RequestConfigData.DomainUri;

                DomainJoinReactively(domainJoinRequest);
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
                    Console.WriteLine("DomainJoinProactively failed:" + ex.HResult);
                }
            }

            Console.WriteLine("Leave DomainJoin.DomainJoinProactively");
        }

        public void DumpDomainJoinServiceRequest(PlayReadyDomainJoinServiceRequest domainJoinRequest)
        {
            if (domainJoinRequest!= null)
            {
                Console.WriteLine("DomainAccountId      = " + domainJoinRequest.DomainAccountId.ToString());
                Console.WriteLine("DomainServiceId      = " + domainJoinRequest.DomainServiceId.ToString());
                Console.WriteLine("DomainFriendlyName   = " + domainJoinRequest.DomainFriendlyName.ToString());
            }
           
        }

        async public void DomainJoinReactively(PlayReadyDomainJoinServiceRequest domainJoinRequest)
        {
            Console.WriteLine("Enter DomainJoin.DomainJoinReactively()");
            Exception exception = null;

            try
            {
                _serviceRequest = domainJoinRequest;

                Console.WriteLine("DomainJoinRequest values before challenge:");
                DumpDomainJoinServiceRequest(domainJoinRequest);

                if (RequestConfigData.ManualEnabling)
                {
                    Console.WriteLine("Manually posting the request...");

                    HttpHelper httpHelper = new HttpHelper(domainJoinRequest);
                    await httpHelper.GenerateChallengeAndProcessResponse();
                }
                else
                {
                    Console.WriteLine("Begin domain join service request...");
                    await domainJoinRequest.BeginServiceRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Saving exception..");
                exception = ex;
            }
            finally
            {
                Console.WriteLine("Post-DomainJoin Values:");
                DumpDomainJoinServiceRequest(domainJoinRequest);
                if (exception == null)
                {
                    Console.WriteLine("ResponseCustomData   = " + domainJoinRequest.ResponseCustomData);
                }

                DomainJoinServiceRequestCompleted(domainJoinRequest, exception);
            }

            Console.WriteLine("Leave DomainJoin.DomainJoinReactively()");
        }
    }

    public class DomainJoinAndReportResult : DomainJoin
    {
        ReportResultDelegate _reportResult = null;
        string _strExpectedError = null;

        public string ExpectedError
        {
            set { this._strExpectedError = value; }
            get { return this._strExpectedError; }
        }

        public DomainJoinAndReportResult(ReportResultDelegate callback)
        {
            _reportResult = callback;
        }

        protected override void DomainJoinServiceRequestCompleted(PlayReadyDomainJoinServiceRequest sender, Exception hrCompletionStatus)
        {
            Console.WriteLine("Enter DomainJoinAndReportResult.DomainJoinServiceRequestCompleted()");

            if (hrCompletionStatus == null)
            {
                Console.WriteLine("***Domain Join succeeded***");
                _reportResult(true, null);
            }
            else
            {
                if (!PerformEnablingActionIfRequested(hrCompletionStatus))
                {
                    Console.WriteLine("DomainJoinServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                    _reportResult(false, null);
                }
            }

            Console.WriteLine("Leave DomainJoinAndReportResult.DomainJoinServiceRequestCompleted()");
        }

        protected override void EnablingActionCompleted(bool bResult)
        {
            Console.WriteLine("Enter DomainJoinAndReportResult.EnablingActionCompleted()");

            _reportResult(bResult, null);

            Console.WriteLine("Leave DomainJoinAndReportResult.EnablingActionCompleted()");
        }

    }

}
