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

    public class Metering : ServiceRequest
    {
        byte[] _meteringCert = null;

        public byte[] GetMeteringCertificate()
        {
            return _meteringCert;
        }

        public void SetMeteringCertificate(byte[] meteringCert)
        {
            _meteringCert = meteringCert;
        }
        protected virtual void MeteringServiceRequestCompleted( PlayReadyMeteringReportServiceRequest  sender, Exception hrCompletionStatus )
        {
            Console.WriteLine("MeteringServiceRequestCompleted");

            if (hrCompletionStatus != null)
            {
                Console.WriteLine("MeteringServiceRequestCompleted failed with " + hrCompletionStatus.HResult);
            }
        }

        void HandleIndivServiceRequest_Finished(bool bResult, object resultContext)
        {
            Console.WriteLine("Enter Revocation.HandleIndivServiceRequest_Finished()");

            Console.WriteLine("HandleIndivServiceRequest_Finished(): " + bResult.ToString());

            if (bResult)
            {
                MeteringReportProactively();
            }

            Console.WriteLine("Leave Revocation.HandleIndivServiceRequest_Finished()");
        }

        public void  MeteringReportProactively()
        {
            Console.WriteLine("Enter Metering.MeteringReportProactively()" );
            try
            {
                Console.WriteLine("Creating metering report service request...");
                PlayReadyMeteringReportServiceRequest meteringRequest = new PlayReadyMeteringReportServiceRequest();
                MeteringReportReactively(meteringRequest);
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
                    Console.WriteLine("MeteringReportProactively failed:" + ex.HResult);
                }
            }

            Console.WriteLine("Leave Metering.MeteringReportProactively()" );
        }

        void ConfigureServiceRequest()
        {
            PlayReadyMeteringReportServiceRequest meteringRequest = _serviceRequest as PlayReadyMeteringReportServiceRequest;
            
            Console.WriteLine(" " );
            Console.WriteLine("Configure metering request to these values:" );
            if( RequestConfigData.Uri != null )
            {
                Console.WriteLine("URL       :" + RequestConfigData.Uri.ToString() );
                meteringRequest.Uri = RequestConfigData.Uri;
            }
            
            if( RequestConfigData.ChallengeCustomData != null && RequestConfigData.ChallengeCustomData != String.Empty )
            {
                Console.WriteLine("ChallengeCustomData:" + RequestConfigData.ChallengeCustomData );
                meteringRequest.ChallengeCustomData = RequestConfigData.ChallengeCustomData;
            }

            meteringRequest.MeteringCertificate = GetMeteringCertificate();

            Console.WriteLine(" ");
        }
        
        async public void  MeteringReportReactively(PlayReadyMeteringReportServiceRequest meteringRequest)
        {
            Console.WriteLine("Enter Metering.MeteringReportReactively()" );
            Exception exception = null;
            
            try
            {   
                _serviceRequest = meteringRequest;
                ConfigureServiceRequest();

                Console.WriteLine("ChallengeCustomData = " + meteringRequest.ChallengeCustomData);
                if( RequestConfigData.ManualEnabling )
                {
                    Console.WriteLine("Manually posting the request..." );
                    
                    HttpHelper httpHelper = new HttpHelper( meteringRequest );
                    await httpHelper.GenerateChallengeAndProcessResponse();
                }
                else
                {
                    Console.WriteLine("Begin metering service request..." );
                    await meteringRequest.BeginServiceRequest();
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine("Saving exception.." );
                exception = ex;
            }
            finally
            {
                Console.WriteLine("Post-Metering Values:");
                if( exception == null )
                {
                    Console.WriteLine("ResponseCustomData = " + meteringRequest.ResponseCustomData);
                    Console.WriteLine("ProtectionSystem   = " + meteringRequest.ProtectionSystem.ToString());
                    Console.WriteLine("Type = " + meteringRequest.Type.ToString());
                }
                
                MeteringServiceRequestCompleted( meteringRequest, exception );
            }
            
            Console.WriteLine("Leave Metering.MeteringReportReactively()" );
        }
        
    }

    public class MeteringAndReportResult : Metering
    {
        ReportResultDelegate _reportResult = null;
        bool _bExpectError = false;
        uint  _expectedPlayCount = 0;
        uint _actualPlayCount = 0;
        public uint PlayCount
        {
            set { this._actualPlayCount = value; }
            get { return this._actualPlayCount; }
        }  

        public MeteringAndReportResult( ReportResultDelegate callback, bool bExpectError , uint expectedPlayCount )
        {
            _reportResult = callback;
            _bExpectError = bExpectError;
            _expectedPlayCount = expectedPlayCount;
        }
        
        protected override void MeteringServiceRequestCompleted( PlayReadyMeteringReportServiceRequest  meteringRequest, Exception hrCompletionStatus )
        {
            Console.WriteLine("Enter MeteringAndReportResult.MeteringServiceRequestCompleted()" );

            if( hrCompletionStatus == null )
            {
                string strMeteringReportXml = XmlConvert.DecodeName( meteringRequest.ResponseCustomData );
                Console.WriteLine("Metering report Xml = " + strMeteringReportXml);

                uint actualPlayCount = 0;
                bool bFound = false;

                if(strMeteringReportXml.Contains("meteringRecord"))
                {
                    //ResponseCustomData format on server http://playready.directtaps.net
                    string [] dataList = strMeteringReportXml.Split(' ');
                    foreach (var data in dataList)
                    {
                        if (data.Contains("Play:"))
                        {
                            bFound = true;
                            string strplayCount = data.Trim().Substring(5);
                            actualPlayCount = Convert.ToUInt32(Regex.Match(strplayCount, @"\d+").Value);
                        }
                    }
                }
                else
                {
                    //otherwise, ResponseCustomData format on server http://capprsvr05/I90playreadymain/rightsmanager.asmx
                    XElement xElement = XElement.Parse(strMeteringReportXml);
                    actualPlayCount = (from item in xElement.Descendants("Action")
                                      where (string)item.Attribute("Name") == "Play"
                                      select (uint)item.Attribute("Value")
                                        ).First();
                    bFound = true;
                }

                if (!bFound)
                {
                    throw new Exception("unrecoganized meteringRequest.ResponseCustomData");
                }

                PlayCount = actualPlayCount;

                if (actualPlayCount == _expectedPlayCount)
                {
                    Console.WriteLine("Actual PlayCount = " + actualPlayCount + " from  metering processed report.");
                    Console.WriteLine("************************************    MeteringReport succeeded       ****************************************");
                   _reportResult( true, null );
                }
                else
                {
                    Console.WriteLine("!!!!!!Actual PlayCount = " + actualPlayCount + "but expected = " + _expectedPlayCount);
                   _reportResult( false, null );
                }
            }
            else
            {
                if( PerformEnablingActionIfRequested(hrCompletionStatus) || HandleExpectedError(hrCompletionStatus))
                {
                    Console.WriteLine( "Exception handled." );
                }
                else
                {
                    Console.WriteLine( "MeteringServiceRequestCompleted ERROR: " + hrCompletionStatus.ToString());
                   _reportResult( false, null );
                }
            }
                
            Console.WriteLine("Leave MeteringAndReportResult.MeteringServiceRequestCompleted()" );
        }
        
        protected override void EnablingActionCompleted(bool bResult)
        {
            Console.WriteLine("Enter MeteringAndReportResult.EnablingActionCompleted()" );

            _reportResult( bResult, null );
            
            Console.WriteLine("Leave MeteringAndReportResult.EnablingActionCompleted()" );
        }

        protected override bool HandleExpectedError(Exception ex)
        {
            Console.WriteLine("Enter MeteringAndReportResult.HandleExpectedError() _bExpectError=" + _bExpectError );
            bool bHandled = false;
            
            if( _bExpectError )
            {
                Console.WriteLine(" ex.HResult= " + ex.HResult );
                if ( ex.HResult == MSPR_E_NO_METERING_DATA_AVAILABLE )
                {
                    Console.WriteLine("MeteringAndReportResult.HandleExpectedError : Received MSPR_E_NO_METERING_DATA_AVAILABLE as expected" );
                    bHandled = true;
                    _reportResult( true, null );
                }
            }
            
            Console.WriteLine("Leave MeteringAndReportResult.HandleExpectedError()" );
            return bHandled;
        }   
    }
}
