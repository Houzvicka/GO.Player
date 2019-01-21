//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Diagnostics;
using Windows.Media.Protection.PlayReady;

namespace GO.UWP.Player.Helpers.Playback
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
            Debug.WriteLine("Enter Indiv.IndivReactively()" );
            Exception exception = null;
            
            try
            {
                _serviceRequest = indivRequest;
                SerivceRequestStatistics.IncIndivCount();

                Debug.WriteLine("Begin indiv service request..." );
                await indivRequest.BeginServiceRequest();
            }
            catch ( Exception ex )
            {
                Debug.WriteLine("Saving exception.." );
                exception = ex;
            }
            finally
            {
                IndivServiceRequestCompleted( indivRequest, exception );
            }
            
            Debug.WriteLine("Leave Indiv.IndivReactively()" );
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
            Debug.WriteLine("Enter IndivAndReportResult.IndivServiceRequestCompleted()" );

            if( hrCompletionStatus == null )
            {
                Debug.WriteLine("***Indiv succeeded***");
                Debug.WriteLine("PlayReady security version " + Windows.Media.Protection.PlayReady.PlayReadyStatics.PlayReadySecurityVersion);
                _reportResult( true, null );
            }
            else
            {
                //needed for LA revoke->Re-Indiv->LA sequence
                if( !PerformEnablingActionIfRequested(hrCompletionStatus) )
                {
                    Debug.WriteLine( "IndivServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                   _reportResult( false, null );
                }
            }
            
            Debug.WriteLine("Leave IndivAndReportResult.IndivServiceRequestCompleted()" );
        }
        
        protected override void EnablingActionCompleted(bool bResult)
        {
            Debug.WriteLine("Enter IndivAndReportResult.EnablingActionCompleted()" );

            _reportResult( bResult, null );
            
            Debug.WriteLine("Leave IndivAndReportResult.EnablingActionCompleted()" );
        }
        
    }

}
