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
    public delegate void ReportResultDelegate( bool bResult, object resultContext );

    public class Indiv :ServiceRequest
    {
        protected virtual void IndivServiceRequestCompleted( PlayReadyIndividualizationServiceRequest  sender, Exception hrCompletionStatus ) 
        {
        }

        public void  IndivProactively()
        {
            PlayReadyIndividualizationServiceRequest indivRequest = new PlayReadyIndividualizationServiceRequest();
            IndivReactively(indivRequest);
        }
        public async void IndivReactively(PlayReadyIndividualizationServiceRequest indivRequest)
        {
            Console.WriteLine("Enter Indiv.IndivReactively()" );
            Exception exception = null;
            
            try
            {
                _serviceRequest = indivRequest;
                SerivceRequestStatistics.IncIndivCount();

                Console.WriteLine("Begin indiv service request..." );
                await indivRequest.BeginServiceRequest();
            }
            catch ( Exception ex )
            {
                Console.WriteLine("Saving exception.." );
                exception = ex;
            }
            finally
            {
                IndivServiceRequestCompleted( indivRequest, exception );
            }
            
            Console.WriteLine("Leave Indiv.IndivReactively()" );
        }

    }


    public class IndivAndReportResult : Indiv
    {
        ReportResultDelegate _reportResult = null;
        public IndivAndReportResult( ReportResultDelegate callback)
        {
            _reportResult = callback;
        }
        
        protected override void IndivServiceRequestCompleted( PlayReadyIndividualizationServiceRequest  sender, Exception hrCompletionStatus )
        {
            Console.WriteLine("Enter IndivAndReportResult.IndivServiceRequestCompleted()" );

            if( hrCompletionStatus == null )
            {
                Console.WriteLine("***Indiv succeeded***");
                Console.WriteLine("PlayReady security version " + Windows.Media.Protection.PlayReady.PlayReadyStatics.PlayReadySecurityVersion);
                _reportResult( true, null );
            }
            else
            {
                //needed for LA revoke->Re-Indiv->LA sequence
                if( !PerformEnablingActionIfRequested(hrCompletionStatus) )
                {
                    Console.WriteLine( "IndivServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                   _reportResult( false, null );
                }
            }
            
            Console.WriteLine("Leave IndivAndReportResult.IndivServiceRequestCompleted()" );
        }
        
        protected override void EnablingActionCompleted(bool bResult)
        {
            Console.WriteLine("Enter IndivAndReportResult.EnablingActionCompleted()" );

            _reportResult( bResult, null );
            
            Console.WriteLine("Leave IndivAndReportResult.EnablingActionCompleted()" );
        }
        
    }

}
