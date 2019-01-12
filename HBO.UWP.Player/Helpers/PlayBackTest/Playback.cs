//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.Media.Protection;
using Windows.Media.Protection.PlayReady;
using PlayReadyUAP;

namespace PlayReadyUAP
{
    public class Playback
    {
        private MediaElement _mediaElement = null;
        Windows.Media.MediaExtensionManager _extensions = null;
        private MediaProtectionManager _protectionManager = null;

        const string _prGUID = "{F4637010-03C3-42CD-B932-B48ADF3A6A54}";
        MediaProtectionServiceCompletion _serviceCompletionNotifier = null;

        RequestChain _requestChain = null;
        ServiceRequestConfigData _requestConfigData = null;
        private string _strMediaPath = null;
        protected bool _bplayToEnd = false;

        public Playback(MediaElement mediaElement)
        {
            SetupProtectionManager(mediaElement);
        }
        public MediaProtectionManager mediaProtectionManager
        {
            set { this._protectionManager = value; }
            get { return this._protectionManager; }
        }

        public ServiceRequestConfigData RequestConfigData  
        {  
            set { this._requestConfigData=  value; }  
            get { return this._requestConfigData; } 
        }
        
        public void HookEventHandlers()
        {
            _mediaElement.CurrentStateChanged += new RoutedEventHandler( CurrentStateChanged );
            _mediaElement.MediaEnded += new RoutedEventHandler( MediaEnded );
            _mediaElement.MediaFailed += new ExceptionRoutedEventHandler( MediaFailed );
            _mediaElement.MediaOpened += new RoutedEventHandler( MediaOpened );
        }
        
        public void UnhookEventHandlers()
        {
            _mediaElement.CurrentStateChanged -= new RoutedEventHandler( CurrentStateChanged );
            _mediaElement.MediaEnded -= new RoutedEventHandler( MediaEnded );
            _mediaElement.MediaFailed -= new ExceptionRoutedEventHandler( MediaFailed );
            _mediaElement.MediaOpened -= new RoutedEventHandler( MediaOpened );
        }

        void SetupProtectionManager(MediaElement mediaElement)
        {
            Console.WriteLine("Enter Playback.SetupProtectionManager()");

            _mediaElement = mediaElement;

            Console.WriteLine("Creating protection system mappings...");
            _protectionManager = new MediaProtectionManager();

            _protectionManager.ComponentLoadFailed += new ComponentLoadFailedEventHandler(ProtectionManager_ComponentLoadFailed);
            _protectionManager.ServiceRequested += new ServiceRequestedEventHandler(ProtectionManager_ServiceRequested);

            Console.WriteLine("Creating protection system mappings...");
            //Setup PlayReady as the ProtectionSystem to use by MF. 
            //The code here is mandatory and should be just copied directly over to the app
            Windows.Foundation.Collections.PropertySet cpSystems = new Windows.Foundation.Collections.PropertySet();

            //Indicate to the MF pipeline to use PlayReady's TrustedInput
            cpSystems.Add("{F4637010-03C3-42CD-B932-B48ADF3A6A54}", "Windows.Media.Protection.PlayReady.PlayReadyWinRTTrustedInput");
            _protectionManager.Properties.Add("Windows.Media.Protection.MediaProtectionSystemIdMapping", cpSystems);
            //Use by the media stream source about how to create ITA InitData.
            //See here for more detai: https://msdn.microsoft.com/en-us/library/windows/desktop/aa376846%28v=vs.85%29.aspx
            _protectionManager.Properties.Add("Windows.Media.Protection.MediaProtectionSystemId", "{F4637010-03C3-42CD-B932-B48ADF3A6A54}");

            // Setup the container GUID that's in the PPSH box
            _protectionManager.Properties.Add("Windows.Media.Protection.MediaProtectionContainerGuid", "{9A04F079-9840-4286-AB92-E65BE0885F95}");

            Console.WriteLine("Creating media extension manager...");
            _extensions = new Windows.Media.MediaExtensionManager();

            Console.WriteLine("Registering ByteStreamHandlers for PIFF content");
            _extensions.RegisterByteStreamHandler("Microsoft.Media.AdaptiveStreaming.SmoothByteStreamHandler", ".ism", "text/xml");
            _extensions.RegisterByteStreamHandler("Microsoft.Media.AdaptiveStreaming.SmoothByteStreamHandler", ".ism", "application/vnd.ms-sstr+xml");

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            //Setup Software Override based on app setting
            //By default, PlayReady uses Hardware DRM if the machine support it. However, in case the app still want
            //software behavior, they can set localSettings.Containers["PlayReady"].Values["SoftwareOverride"]=1. 
            //This code tells MF to use software override as well
            if (localSettings.Containers.ContainsKey("PlayReady") &&
                localSettings.Containers["PlayReady"].Values.ContainsKey("SoftwareOverride"))
            {
                int UseSoftwareProtectionLayer = (int) localSettings.Containers["PlayReady"].Values["SoftwareOverride"];

                if(UseSoftwareProtectionLayer  == 1)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("***** Use Software Protection Layer ******");
                    _protectionManager.Properties.Add("Windows.Media.Protection.UseSoftwareProtectionLayer", true);
                }
            }

            _mediaElement.ProtectionManager = _protectionManager;

            Console.WriteLine("Leave Playback.SetProtectionManager()");
        }

        public void SetMediaSource(string strMediaPath,
                                   bool bPlayToEnd)
        {
            Console.WriteLine("Enter Playback.SetMediaSource()");

            if (_mediaElement == null)
            {
                Console.WriteLine("_mediaElement is closed ");
                return;
            }

            _bplayToEnd = bPlayToEnd;
            _strMediaPath = strMediaPath;

            //
            // In order to play Smooth Streaming content (e.g. PIFF_SuperSpeedway_720.ism at
            // http://playready.directtaps.net/smoothstreaming/SSWSS720H264PR/SuperSpeedway_720.ism/Manifest)
            // you need to add Microsoft Universal Smooth Streaming Client SDK to the project's References.
            // If you haven't installed the SDK, you can download and install the SDK from
            // https://visualstudiogallery.msdn.microsoft.com/1e7d4700-7fa8-49b6-8a7b-8d8666685459?SRC=Home.
            // Also add Visual C++ 2015 Runtime for Universal Windows Platform Apps to the project's References
            // if it hasn't been added.
            //
            HookEventHandlers();
            _mediaElement.Source = new Uri(_strMediaPath);

            Console.WriteLine("Leave Playback.SetMediaSource()");
        }

        public void LoadMedia(MediaElement mediaElement,
                                string strMediaPath,
                                bool bPlayToEnd)
        {
            Console.WriteLine("Enter Playback.LoadMedia()");

            _bplayToEnd = bPlayToEnd;
            _mediaElement = mediaElement;
            _strMediaPath = strMediaPath;

            SetMediaSource(strMediaPath, bPlayToEnd);

            Console.WriteLine("Leave Playback.LoadMedia()");
        }

        public void FullPlayback(MediaElement mediaElement, string strMediaPath)
        {
            Console.WriteLine("Enter Playback.Play()");

            HookEventHandlers();            
            SetMediaSource(strMediaPath, true);
            
            if (_mediaElement == null)
            {
                Console.WriteLine("mediaElement is closed ");
                return;
            }
            else
            {
                _mediaElement.Play();
            }

            Console.WriteLine("Leave Playback.Play()");
        }

        public void Play(bool bPlayToEnd)
        {
            _bplayToEnd = bPlayToEnd;
            Console.WriteLine("Enter Playback.Play()");
            if (_mediaElement == null)
            {
                Console.WriteLine("_mediaElement is closed ");
                return;
            }
            else
            {
                _mediaElement.Play();
            }
            Console.WriteLine("Leave Playback.Play()");
        }

        public void Stop()
        {
            Console.WriteLine("Enter Playback.Stop()");
            if (_mediaElement == null)
            {
                Console.WriteLine("_mediaElement is closed ");
                return;
            }
            else
            {
                _mediaElement.Stop();
            }
            Console.WriteLine("Leave Playback.Stop()");
        }

        public void Pause()
        {
            Console.WriteLine("Enter Playback.Pause()");
            if (_mediaElement == null)
            {
                Console.WriteLine("_mediaElement is closed ");
                return;
            }
            else
            {
                _mediaElement.Pause();
            }
            Console.WriteLine("Leave Playback.Pause()");
        }

        public void Play(string strMediaPath)
        {
            Console.WriteLine("Enter Playback.Play()" );
            Console.WriteLine(strMediaPath);

            _strMediaPath = strMediaPath;

            HookEventHandlers();
            _mediaElement.Source = new Uri( _strMediaPath );
            
            Console.WriteLine("Leave Playback.Play()" );
        }

        protected void CurrentStateChanged( object sender, RoutedEventArgs e )
        {
            Console.WriteLine("CurrentState:" + ((MediaElement)sender).CurrentState);
        }

        virtual protected void MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            UnhookEventHandlers();
            Console.WriteLine("MediaFailed Source: " + ((MediaElement)sender).Source );
            Console.WriteLine("Playback Failed");
            Console.WriteLine("MediaFailed: " + e.ErrorMessage );
        }

        virtual protected void MediaEnded( object sender, RoutedEventArgs e )
        {
            UnhookEventHandlers();
            Console.WriteLine("MediaEnded: " + ((MediaElement)sender).Source );
            Console.WriteLine("Playback succeeded");
        }

        virtual protected void MediaOpened( object sender, RoutedEventArgs e )
        {
            Console.WriteLine("MediaOpened: " + ((MediaElement)sender).Source );
        }

        void ProtectionManager_ComponentLoadFailed( MediaProtectionManager sender, ComponentLoadFailedEventArgs e )
        {
            Console.WriteLine("Enter Playback.ProtectionManager_ComponentLoadFailed()" );
            Console.WriteLine( e.Information.ToString() );
            
            //  List the failing components - RevocationAndRenewalInformation
            for ( int i = 0; i < e.Information.Items.Count; i++ )
            {
                Console.WriteLine(e.Information.Items[i].Name + "\nReasons=0x" + e.Information.Items[i].Reasons + "\n"
                                                    + "Renewal Id=" + e.Information.Items[i].RenewalId );

            }
            e.Completion.Complete( false );
            Console.WriteLine("Leave Playback.ProtectionManager_ComponentLoadFailed()" );
        }

        void ProtectionManager_ServiceRequested( MediaProtectionManager sender, ServiceRequestedEventArgs srEvent )
        {
            Console.WriteLine("Enter Playback.ProtectionManager_ServiceRequested()" );
            
            _serviceCompletionNotifier = srEvent.Completion;
            IPlayReadyServiceRequest serviceRequest = ( IPlayReadyServiceRequest )srEvent.Request;
            Console.WriteLine("Servie request type = " + serviceRequest.GetType());

            _requestChain = new RequestChain( serviceRequest );
            _requestChain.RequestConfigData = this.RequestConfigData;
            _requestChain.FinishAndReportResult( new ReportResultDelegate(HandleServiceRequest_Finished));
            
            Console.WriteLine("Leave Playback.ProtectionManager_ServiceRequested()" );
        }

        void HandleServiceRequest_Finished(bool bResult, object resultContext)
        {
            Console.WriteLine("Enter Playback.HandleServiceRequest_Finished()" );
            
            Console.WriteLine("MediaProtectionServiceCompletion.Complete = " + bResult.ToString() );
            _serviceCompletionNotifier.Complete( bResult );
            
            Console.WriteLine("Leave Playback.HandleServiceRequest_Finished()" );
        }
        
    }

    public class PlaybackAndReportResult: Playback
    {
        ReportResultDelegate _reportResult = null;
        string _strExpectedError = null;

        public PlaybackAndReportResult(MediaElement mediaElement, ReportResultDelegate callback, string strExpectedError ): base(mediaElement)
        {
            _reportResult       = callback;
            _strExpectedError   = strExpectedError;
        }

        public PlaybackAndReportResult(MediaElement mediaElement, ReportResultDelegate callback) : base(mediaElement)
        {
            _reportResult = callback;
            _strExpectedError = null;
        }

        override protected void MediaOpened(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Enter PlaybackAndReportResult.MediaOpened()");

            base.MediaOpened(sender, e);
            
            Console.WriteLine("!!!!debug:_bplayToEnd =" + _bplayToEnd );
            
            if (!_bplayToEnd)
            {
                _reportResult(true, null);
            }

            Console.WriteLine("Leave PlaybackAndReportResult.MediaOpened()");
        }

        override protected void MediaEnded( object sender, RoutedEventArgs e )
        {
            Console.WriteLine("Enter PlaybackAndReportResult.MediaEnded()" );
                        
            base.MediaEnded(sender, e);
            if (_bplayToEnd)
            {
                _reportResult(true, null);
            }

            Console.WriteLine("Leave PlaybackAndReportResult.MediaEnded()" );
        }
        
        override protected void MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine("Enter PlaybackAndReportResult.MediaFailed()" );
            
            base.MediaFailed(sender, e);
            
            bool bHandled = false;
            if( _strExpectedError != null )
            {
                if ( e.ErrorMessage.ToLower().Contains( _strExpectedError.ToLower() ) )
                {
                    Console.WriteLine( "'" + e.ErrorMessage + "' Contains " + _strExpectedError + "  as expected" );
                    bHandled = true;
                }
            }
            _reportResult( bHandled, null );
            
            Console.WriteLine("Leave PlaybackAndReportResult.MediaFailed()" );
        }
        
    }


}

