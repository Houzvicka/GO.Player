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
    public class Revocation :ServiceRequest
    {
        protected virtual void RevocationServiceRequestCompleted( PlayReadyRevocationServiceRequest  sender, Exception hrCompletionStatus ) 
        {
            Console.WriteLine("RevocationServiceRequestCompleted");

            if (hrCompletionStatus != null)
            {
                Console.WriteLine("RevocationServiceRequestCompleted failed with " + hrCompletionStatus.HResult);
            }
        }

        void HandleIndivServiceRequest_Finished(bool bResult, object resultContext)
        {
            Console.WriteLine("Enter Revocation.HandleIndivServiceRequest_Finished()");

            Console.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());
            if (bResult)
            {
                HandleRevocationProactively();
            }

            Console.WriteLine("Leave Revocation.HandleIndivServiceRequest_Finished()");
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
                    Console.WriteLine("HandleRevocationProactively failed:" + ex.HResult);
                }
            }
        }
        async public void  HandleRevocationReactively(PlayReadyRevocationServiceRequest request)
        {
            Console.WriteLine("Enter Revocation.HandleRevocationReactively()" );
            Exception exception = null;
            
            try
            {
                _serviceRequest = request;

                Console.WriteLine("Begin revocation service request..." );
                await request.BeginServiceRequest();
            }
            catch ( Exception ex )
            {
                Console.WriteLine("Saving exception.." );
                exception = ex;
            }
            finally
            {
                Console.WriteLine("Post-RevocationServiceRequest Values:");
                if( exception == null )
                {
                    Console.WriteLine("ResponseCustomData = " + request.ResponseCustomData);
                    Console.WriteLine("ProtectionSystem   = " + request.ProtectionSystem.ToString());
                    Console.WriteLine("Type = " + request.Type.ToString());
                }
                
                RevocationServiceRequestCompleted( request, exception );
            }
            
            Console.WriteLine("Leave Revocation.HandleRevocationReactively()" );
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
            Console.WriteLine("Enter RevocationAndReportResult.RevocationServiceRequestCompleted()" );

            if( hrCompletionStatus == null )
            {
                Console.WriteLine("********************************************Revocation handling succeeded**************************************************");
                _reportResult( true, null );
            }
            else
            {
                if( !PerformEnablingActionIfRequested(hrCompletionStatus) )
                {
                    Console.WriteLine( "RevocationServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                   _reportResult( false, null );
                }
            }
            
            Console.WriteLine("Leave RevocationAndReportResult.RevocationServiceRequestCompleted()" );
        }
        
        protected override void EnablingActionCompleted(bool bResult)
        {
            Console.WriteLine("Enter RevocationAndReportResult.EnablingActionCompleted()" );

            _reportResult( bResult, null );
            
            Console.WriteLine("Leave RevocationAndReportResult.EnablingActionCompleted()" );
        }
        
    }

}
