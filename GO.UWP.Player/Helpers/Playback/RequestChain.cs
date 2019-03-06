//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System.Diagnostics;
using Windows.Media.Protection.PlayReady;

namespace GO.UWP.Player.Helpers.Playback
{
    public class RequestChain
    {
        private IPlayReadyServiceRequest _serviceRequest = null;
        ReportResultDelegate _reportResult = null;

        IndivAndReportResult _indivAndReportResult  = null;
        LAAndReportResult _licenseAcquisition       = null;
        RevocationAndReportResult _revocationAndReportResult = null;

        ServiceRequestConfigData _requestConfigData = null;
        public ServiceRequestConfigData RequestConfigData  
        {  
            set { this._requestConfigData=  value; }  
            get { return this._requestConfigData; } 
        }

        public RequestChain( IPlayReadyServiceRequest serviceRequest)
        {
            _serviceRequest = serviceRequest;
        }

        public void FinishAndReportResult(ReportResultDelegate callback)
        {
            _reportResult = callback;
            HandleServiceRequest();
        }
        
        void HandleServiceRequest()
        {
            if( _serviceRequest is PlayReadyIndividualizationServiceRequest )
            {
                HandleIndivServiceRequest( (PlayReadyIndividualizationServiceRequest)_serviceRequest);
            }
            else if ( _serviceRequest is PlayReadyLicenseAcquisitionServiceRequest )
            {
                HandleLicenseAcquisitionServiceRequest((PlayReadyLicenseAcquisitionServiceRequest)_serviceRequest);
            }
            else if ( _serviceRequest is PlayReadyRevocationServiceRequest )
            {
                HandleRevocationServiceRequest((PlayReadyRevocationServiceRequest)_serviceRequest);
            }
            else
            {
                Debug.WriteLine("ERROR: Unsupported serviceRequest " + _serviceRequest.GetType() );
            }
        }
        
        void HandleServiceRequest_Finished(bool bResult, object resultContext)
        {
            Debug.WriteLine("Enter RequestChain.HandleServiceRequest_Finished()" );
            
            _reportResult( bResult, null );
            
            Debug.WriteLine("Leave RequestChain.HandleServiceRequest_Finished()" );
        }

        void HandleIndivServiceRequest(PlayReadyIndividualizationServiceRequest serviceRequest)
        {
            Debug.WriteLine(" " );
            Debug.WriteLine("Enter RequestChain.HandleIndivServiceRequest()" );
            
            _indivAndReportResult = new IndivAndReportResult( new ReportResultDelegate(HandleServiceRequest_Finished));
            _indivAndReportResult.RequestConfigData = _requestConfigData;
            _indivAndReportResult.IndivReactively( serviceRequest );
            
            Debug.WriteLine("Leave RequestChain.HandleIndivServiceRequest()" );
        }

        void HandleLicenseAcquisitionServiceRequest(PlayReadyLicenseAcquisitionServiceRequest serviceRequest)
        {
            Debug.WriteLine(" " );
            Debug.WriteLine("Enter RequestChain.HandleLicenseAcquisitionServiceRequest()" );
            
            _licenseAcquisition = new LAAndReportResult( new ReportResultDelegate(HandleServiceRequest_Finished));
            _licenseAcquisition.RequestConfigData = _requestConfigData;
            _licenseAcquisition.AcquireLicenseReactively( serviceRequest);
            //_licenseAcquisition.RequestLicenseManual(serviceRequest);

            Debug.WriteLine("Leave RequestChain.HandleLicenseAcquisitionServiceRequest()" );
        }

        void HandleRevocationServiceRequest(PlayReadyRevocationServiceRequest serviceRequest)
        {
            Debug.WriteLine(" ");
            Debug.WriteLine("Enter RequestChain.HandleRevocationServiceRequest()");

            _revocationAndReportResult = new RevocationAndReportResult(new ReportResultDelegate(HandleServiceRequest_Finished));
            _revocationAndReportResult.RequestConfigData = _requestConfigData;
            _revocationAndReportResult.HandleRevocationReactively(serviceRequest);

            Debug.WriteLine("Leave RequestChain.HandleRevocationServiceRequest()");
        }
    }
}
