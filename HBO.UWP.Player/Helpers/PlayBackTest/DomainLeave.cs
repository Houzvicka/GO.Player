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
    public class DomainLeave : ServiceRequest
    {

        protected virtual void DomainLeaveServiceRequestCompleted(PlayReadyDomainLeaveServiceRequest sender, Exception hrCompletionStatus)
        {
            Console.WriteLine("DomainLeaveServiceRequestCompleted");

            if (hrCompletionStatus != null)
            {
                Console.WriteLine("DomainLeaveServiceRequestCompleted failed with " + hrCompletionStatus.HResult);
            }
        }

        void HandleIndivServiceRequest_Finished(bool bResult, object resultContext )
        {
            Console.WriteLine("Enter DomainLeave.HandleIndivServiceRequest_Finished()");

            Console.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());
            if (bResult)
            {
                DomainLeaveProactively();
            }

            Console.WriteLine("Leave DomainLeave.HandleIndivServiceRequest_Finished()");
        }

        public void DomainLeaveProactively()
        {
            Console.WriteLine("Enter DomainLeave.DomainLeaveProactively()");
            try
            {
                PlayReadyDomainLeaveServiceRequest domainLeaveRequest = new PlayReadyDomainLeaveServiceRequest();

                DomainLeaveReactively(domainLeaveRequest);
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
                    Console.WriteLine("DomainLeaveProactively failed:" + ex.HResult);
                }
            }

            Console.WriteLine("Leave DomainLeave.DomainLeaveProactively()");
        }

        async public void DomainLeaveReactively(PlayReadyDomainLeaveServiceRequest domainLeaveRequest)
        {
            Console.WriteLine("Enter DomainLeave.DomainLeaveReactively()");
            Exception exception = null;

            try
            {
                _serviceRequest = domainLeaveRequest;
                domainLeaveRequest.DomainServiceId = RequestConfigData.DomainServiceId;
                domainLeaveRequest.DomainAccountId = RequestConfigData.DomainAccountId;
                domainLeaveRequest.Uri = RequestConfigData.DomainUri;

                if (RequestConfigData.ManualEnabling)
                {
                    Console.WriteLine("Manually posting the request...");

                    HttpHelper httpHelper = new HttpHelper(domainLeaveRequest);
                    await httpHelper.GenerateChallengeAndProcessResponse();
                }
                else
                {
                    Console.WriteLine("Begin domain leave service request...");
                    await domainLeaveRequest.BeginServiceRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Saving exception..");
                exception = ex;
            }
            finally
            {
                DomainLeaveServiceRequestCompleted(domainLeaveRequest, exception);
            }

            Console.WriteLine("Leave DomainLeave.DomainLeaveReactively()");
        }
    }

    public class DomainLeaveAndReportResult : DomainLeave
    {
        ReportResultDelegate _reportResult = null;
        string _strExpectedError = null;

        public string ExpectedError
        {
            set { this._strExpectedError = value; }
            get { return this._strExpectedError; }
        }

        public DomainLeaveAndReportResult(ReportResultDelegate callback)
        {
            _reportResult = callback;
        }

        protected override void DomainLeaveServiceRequestCompleted(PlayReadyDomainLeaveServiceRequest sender, Exception hrCompletionStatus)
        {
            Console.WriteLine("Enter DomainLeaveAndReportResult.DomainLeaveServiceRequestCompleted()");

            if (hrCompletionStatus == null)
            {
                Console.WriteLine("***Domain Leave succeeded***");
                _reportResult(true, null);
            }
            else
            {
                if (!PerformEnablingActionIfRequested(hrCompletionStatus))
                {
                    Console.WriteLine("DomainLeaveServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                    _reportResult(false, null);
                }
            }

            Console.WriteLine("Leave DomainLeaveAndReportResult.DomainLeaveServiceRequestCompleted()");
        }

        protected override void EnablingActionCompleted(bool bResult)
        {
            Console.WriteLine("Enter DomainLeaveAndReportResult.EnablingActionCompleted()");

            _reportResult(bResult, null);

            Console.WriteLine("Leave DomainLeaveAndReportResult.EnablingActionCompleted()");
        }

    }

}
