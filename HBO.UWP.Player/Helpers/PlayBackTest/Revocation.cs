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
    public class Revocation :ServiceRequest
    {
        protected virtual void RevocationServiceRequestCompleted( PlayReadyRevocationServiceRequest  sender, Exception hrCompletionStatus ) 
        {
            Debug.WriteLine("RevocationServiceRequestCompleted");

            if (hrCompletionStatus != null)
            {
                Debug.WriteLine("RevocationServiceRequestCompleted failed with " + hrCompletionStatus.HResult);
            }
        }

        void HandleIndivServiceRequest_Finished(bool bResult, object resultContext)
        {
            Debug.WriteLine("Enter Revocation.HandleIndivServiceRequest_Finished()");

            Debug.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());
            if (bResult)
            {
                HandleRevocationProactively();
            }

            Debug.WriteLine("Leave Revocation.HandleIndivServiceRequest_Finished()");
        }

        public void  HandleRevocationProactively()
        {
            try
            {
                PlayReadyRevocationServiceRequest request = new PlayReadyRevocationServiceRequest();
                HandleRevocationReactively(request);
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
                    Debug.WriteLine("HandleRevocationProactively failed:" + ex.HResult);
                }
            }
        }
        async public void  HandleRevocationReactively(PlayReadyRevocationServiceRequest request)
        {
            Debug.WriteLine("Enter Revocation.HandleRevocationReactively()" );
            Exception exception = null;
            
            try
            {
                _serviceRequest = request;

                Debug.WriteLine("Begin revocation service request..." );
                await request.BeginServiceRequest();
            }
            catch ( Exception ex )
            {
                Debug.WriteLine("Saving exception.." );
                exception = ex;
            }
            finally
            {
                Debug.WriteLine("Post-RevocationServiceRequest Values:");
                if( exception == null )
                {
                    Debug.WriteLine("ResponseCustomData = " + request.ResponseCustomData);
                    Debug.WriteLine("ProtectionSystem   = " + request.ProtectionSystem.ToString());
                    Debug.WriteLine("Type = " + request.Type.ToString());
                }
                
                RevocationServiceRequestCompleted( request, exception );
            }
            
            Debug.WriteLine("Leave Revocation.HandleRevocationReactively()" );
        }

    }

    public class RevocationAndReportResult : Revocation
    {
        ReportResultDelegate _reportResult = null;
        public RevocationAndReportResult( ReportResultDelegate callback)
        {
            _reportResult = callback;
        }
        
        protected override void RevocationServiceRequestCompleted( PlayReadyRevocationServiceRequest  sender, Exception hrCompletionStatus )
        {
            Debug.WriteLine("Enter RevocationAndReportResult.RevocationServiceRequestCompleted()" );

            if( hrCompletionStatus == null )
            {
                Debug.WriteLine("********************************************Revocation handling succeeded**************************************************");
                _reportResult( true, null );
            }
            else
            {
                if( !PerformEnablingActionIfRequested(hrCompletionStatus) )
                {
                    Debug.WriteLine( "RevocationServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                   _reportResult( false, null );
                }
            }
            
            Debug.WriteLine("Leave RevocationAndReportResult.RevocationServiceRequestCompleted()" );
        }
        
        protected override void EnablingActionCompleted(bool bResult)
        {
            Debug.WriteLine("Enter RevocationAndReportResult.EnablingActionCompleted()" );

            _reportResult( bResult, null );
            
            Debug.WriteLine("Leave RevocationAndReportResult.EnablingActionCompleted()" );
        }
        
    }

}
