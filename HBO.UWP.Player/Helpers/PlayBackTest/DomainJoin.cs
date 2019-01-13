//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Media.Protection.PlayReady;
using PlayReadyUAP;

namespace PlayReadyUAP
{
    public class DomainJoin : ServiceRequest
    {

        protected virtual void DomainJoinServiceRequestCompleted(PlayReadyDomainJoinServiceRequest sender, Exception hrCompletionStatus)
        {
            Debug.WriteLine("DomainJoinServiceRequestCompleted");

            if (hrCompletionStatus != null)
            {
                Debug.WriteLine("DomainJoinServiceRequestCompleted failed with " + hrCompletionStatus.HResult);
            }
        }

        void HandleIndivServiceRequest_Finished(bool bResult, object resultContext)
        {
            Debug.WriteLine("Enter DomainJoin.HandleIndivServiceRequest_Finished()");

            Debug.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());
            if (bResult)
            {
                DomainJoinProactively();
            }

            Debug.WriteLine("Leave DomainJoin.HandleIndivServiceRequest_Finished()");
        }

        public void DomainJoinProactively()
        {
            Debug.WriteLine("Enter DomainJoin.DomainJoinProactively()");
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
                    Debug.WriteLine("DomainJoinProactively failed:" + ex.HResult);
                }
            }

            Debug.WriteLine("Leave DomainJoin.DomainJoinProactively");
        }

        public void DumpDomainJoinServiceRequest(PlayReadyDomainJoinServiceRequest domainJoinRequest)
        {
            if (domainJoinRequest!= null)
            {
                Debug.WriteLine("DomainAccountId      = " + domainJoinRequest.DomainAccountId.ToString());
                Debug.WriteLine("DomainServiceId      = " + domainJoinRequest.DomainServiceId.ToString());
                Debug.WriteLine("DomainFriendlyName   = " + domainJoinRequest.DomainFriendlyName.ToString());
            }
           
        }

        async public void DomainJoinReactively(PlayReadyDomainJoinServiceRequest domainJoinRequest)
        {
            Debug.WriteLine("Enter DomainJoin.DomainJoinReactively()");
            Exception exception = null;

            try
            {
                _serviceRequest = domainJoinRequest;

                Debug.WriteLine("DomainJoinRequest values before challenge:");
                DumpDomainJoinServiceRequest(domainJoinRequest);

                if (RequestConfigData.ManualEnabling)
                {
                    Debug.WriteLine("Manually posting the request...");

                    HttpHelper httpHelper = new HttpHelper(domainJoinRequest);
                    await httpHelper.GenerateChallengeAndProcessResponse();
                }
                else
                {
                    Debug.WriteLine("Begin domain join service request...");
                    await domainJoinRequest.BeginServiceRequest();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Saving exception..");
                exception = ex;
            }
            finally
            {
                Debug.WriteLine("Post-DomainJoin Values:");
                DumpDomainJoinServiceRequest(domainJoinRequest);
                if (exception == null)
                {
                    Debug.WriteLine("ResponseCustomData   = " + domainJoinRequest.ResponseCustomData);
                }

                DomainJoinServiceRequestCompleted(domainJoinRequest, exception);
            }

            Debug.WriteLine("Leave DomainJoin.DomainJoinReactively()");
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
            Debug.WriteLine("Enter DomainJoinAndReportResult.DomainJoinServiceRequestCompleted()");

            if (hrCompletionStatus == null)
            {
                Debug.WriteLine("***Domain Join succeeded***");
                _reportResult(true, null);
            }
            else
            {
                if (!PerformEnablingActionIfRequested(hrCompletionStatus))
                {
                    Debug.WriteLine("DomainJoinServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                    _reportResult(false, null);
                }
            }

            Debug.WriteLine("Leave DomainJoinAndReportResult.DomainJoinServiceRequestCompleted()");
        }

        protected override void EnablingActionCompleted(bool bResult)
        {
            Debug.WriteLine("Enter DomainJoinAndReportResult.EnablingActionCompleted()");

            _reportResult(bResult, null);

            Debug.WriteLine("Leave DomainJoinAndReportResult.EnablingActionCompleted()");
        }

    }

}
