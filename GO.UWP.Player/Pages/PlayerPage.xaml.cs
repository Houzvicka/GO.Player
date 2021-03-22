using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Protection;
using Windows.Media.Protection.PlayReady;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GO.UWP.Player.Helpers;
using GO.UWP.Player.Helpers.Playback;
using GO.UWP.Player.Messages;
using GO.UWP.Player.Model;
using GO.UWP.Player.ViewModel;
using Microsoft.PlayerFramework.Adaptive;
using Microsoft.PlayerFramework.TimedText;
using Microsoft.PlayerFramework;

namespace GO.UWP.Player.Pages
{
    public sealed partial class PlayerPage : Page
    {
        private MainViewModel mvm => (MainViewModel)DataContext;
        
        MediaProtectionManager mediaProtectionManager = new MediaProtectionManager();

        public PlayerPage()
        {
            this.InitializeComponent();
        }

        public void SetupRequestConfigData(Guid customerId, Purchase purchase)
        {
            RequestConfigData = new ServiceRequestConfigData()
            {
                Uri = new Uri($"https://lic.drmtoday.com/license-proxy-headerauth/drmtoday/RightsManager.asmx?assetId={purchase.AssetId}&variantId={purchase.VariantId}"),
                ChallengeCustomData = Base64Helpers.Base64Encode("{\"userId\":\"" + customerId + "\",\"sessionId\":\"" + purchase.PlayerSessionId + "\",\"merchant\":\"hboeurope\"}"),
                CustomArtibutes = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("x-dt-auth-token", purchase.AuthToken),
                    new KeyValuePair<string, string>("Origin", "https://www.hbogo.cz"),
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetupRequestConfigData(mvm.CurrentUser.Customer.Id, mvm.CurrentlySelectedVideo.Purchase);
            var playUri = new Uri(mvm.CurrentlySelectedVideo.Purchase.MediaUrl.AbsoluteUri + "/manifest");

            Player.Source = new Uri(playUri.ToString());

            InitializePlugins();
            InitializeMediaExtensionManager();
            InitializeMediaProtectionManager();

            HookEventHandlers();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Dispose();
        }

        private void HookEventHandlers()
        {
            Player.CurrentStateChanged += new RoutedEventHandler(CurrentStateChanged);
            Player.MediaEnded += MediaEnded;
            Player.MediaFailed += new ExceptionRoutedEventHandler(MediaFailed);
            Player.MediaOpened += new RoutedEventHandler(MediaOpened);
        }

        private void UnhookEventHandlers()
        {
            Player.CurrentStateChanged -= new RoutedEventHandler(CurrentStateChanged);
            Player.MediaEnded -= MediaEnded;
            Player.MediaFailed -= new ExceptionRoutedEventHandler(MediaFailed);
            Player.MediaOpened -= new RoutedEventHandler(MediaOpened);
        }

        private void MediaOpened(object sender, RoutedEventArgs e)
        {
            //DebugLogger.Log("MediaOpened: " + ((MediaPlayer)sender).Source);
            // Start playing the file when ready
            Player.Play();
        }

        private void CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            //DebugLogger.Log("CurrentState:" + ((MediaPlayer)sender).CurrentState);
        }

        private void MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            UnhookEventHandlers();
            //DebugLogger.Log("MediaFailed Source: " + ((MediaPlayer)sender).Source);
            //DebugLogger.Log("Playback Failed");
            //DebugLogger.Log("MediaFailed: " + e.ErrorMessage);
        }

        private void MediaEnded(object sender, MediaPlayerActionEventArgs e)
        {
            UnhookEventHandlers();
            //DebugLogger.Log("MediaEnded: " + ((MediaPlayer)sender).Source);
            //DebugLogger.Log("Playback succeeded");
        }

        public void Dispose()
        {
            UnhookEventHandlers();

            mediaProtectionManager.ComponentLoadFailed -= new ComponentLoadFailedEventHandler(OnMediaProtectionManagerComponentLoadFailed);
            mediaProtectionManager.ServiceRequested -= new ServiceRequestedEventHandler(OnMediaProtectionManagerServiceRequested);
            
            if (Player != null)
            {
                Player.Close();
                //Player = null;
            }
        }

        /// <summary>Initializes the Smooth Streaming plugin.</summary>
        private void InitializePlugins()
        {
            var adaptivePlugin = new AdaptivePlugin() { InstreamCaptionsEnabled = true };
            Player.Plugins.Add(adaptivePlugin);

            var captionPlugin = new CaptionsPlugin();
            Player.Plugins.Add(captionPlugin);
        }

        /// <summary>Initializes the media extension manager so we can handle PlayReady protected content.</summary>
        private void InitializeMediaExtensionManager()
        {
            var plugins = new MediaExtensionManager();

            // Add support for IIS Smooth Streaming Manifests
            plugins.RegisterByteStreamHandler("Microsoft.Media.AdaptiveStreaming.SmoothByteStreamHandler", ".ism", "text/xml");
            plugins.RegisterByteStreamHandler("Microsoft.Media.AdaptiveStreaming.SmoothByteStreamHandler", ".ism", "application/vnd.ms-sstr+xml");

            // Add support for PlayReady video and audio files
            plugins.RegisterByteStreamHandler("Microsoft.Media.Protection.PlayReady.PlayReadyByteStreamHandler", ".pyv", "");
            plugins.RegisterByteStreamHandler("Microsoft.Media.Protection.PlayReady.PlayReadyByteStreamHandler", ".pya", "");
        }

        /// <summary>Initializes the PlayReady protection manager.</summary>
        private void InitializeMediaProtectionManager()
        {
            mediaProtectionManager.Properties.Clear();
            mediaProtectionManager.ComponentLoadFailed += OnMediaProtectionManagerComponentLoadFailed;
            mediaProtectionManager.ServiceRequested += OnMediaProtectionManagerServiceRequested;

            // Set up the container GUID for the CFF format (used with DASH streams), see http://uvdemystified.com/uvfaq.html#3.2
            // The GUID represents MPEG DASH Content Protection using Microsoft PlayReady, see http://dashif.org/identifiers/protection/
            mediaProtectionManager.Properties["Windows.Media.Protection.MediaProtectionContainerGuid"] = "{9A04F079-9840-4286-AB92-E65BE0885F95}";

            // Set up the drm layer to use. Hardware DRM is the default, but not all older hardware supports this
            var supportsHardwareDrm = PlayReadyStatics.CheckSupportedHardware(PlayReadyHardwareDRMFeatures.HardwareDRM);
            if (!supportsHardwareDrm)
            {
                mediaProtectionManager.Properties["Windows.Media.Protection.UseSoftwareProtectionLayer"] = true;
            }

            // Set up the content protection manager so it uses the PlayReady Input Trust Authority (ITA) for the relevant media sources
            // The MediaProtectionSystemId GUID is format and case sensitive, see https://msdn.microsoft.com/en-us/library/windows.media.protection.mediaprotectionmanager.properties.aspx
            var cpsystems = new PropertySet();
            cpsystems[PlayReadyStatics.MediaProtectionSystemId.ToString("B").ToUpper()] = "Windows.Media.Protection.PlayReady.PlayReadyWinRTTrustedInput";
            mediaProtectionManager.Properties["Windows.Media.Protection.MediaProtectionSystemIdMapping"] = cpsystems;
            mediaProtectionManager.Properties["Windows.Media.Protection.MediaProtectionSystemId"] = PlayReadyStatics.MediaProtectionSystemId.ToString("B").ToUpper();

            Player.ProtectionManager = mediaProtectionManager;
        }

        private void OnMediaProtectionManagerComponentLoadFailed(MediaProtectionManager sender, ComponentLoadFailedEventArgs e)
        {
            Debug.WriteLine("ProtectionManager ComponentLoadFailed");
            e.Completion.Complete(false);
        }

        public ServiceRequestConfigData RequestConfigData
        {
            set { this._requestConfigData = value; }
            get { return this._requestConfigData; }
        }

        RequestChain _requestChain = null;
        ServiceRequestConfigData _requestConfigData = null;
        MediaProtectionServiceCompletion _serviceCompletionNotifier = null;

        private async void OnMediaProtectionManagerServiceRequested(MediaProtectionManager sender, ServiceRequestedEventArgs e)
        {
            Debug.WriteLine("ProtectionManager ServiceRequested");

            _serviceCompletionNotifier = e.Completion;
            IPlayReadyServiceRequest serviceRequest = (IPlayReadyServiceRequest)e.Request;
            Debug.WriteLine("Servie request type = " + serviceRequest.GetType());

            var result = false;

            if (serviceRequest.Type == PlayReadyStatics.IndividualizationServiceRequestType)
            {
                result = await PlayReadyLicenseHandler.RequestIndividualizationToken(serviceRequest as PlayReadyIndividualizationServiceRequest);
            }
            else if (serviceRequest.Type == PlayReadyStatics.LicenseAcquirerServiceRequestType)
            {
                // NOTE: You might need to set the request.ChallengeCustomData, depending on your Rights Manager.
                if (RequestConfigData != null)
                {
                    _requestChain = new RequestChain(serviceRequest);
                    _requestChain.RequestConfigData = this.RequestConfigData;
                    _requestChain.FinishAndReportResult(HandleServiceRequest_Finished);

                    return;
                }
                else result = await PlayReadyLicenseHandler.RequestLicense(serviceRequest as PlayReadyLicenseAcquisitionServiceRequest);
            }

            _serviceCompletionNotifier.Complete(result);
        }

        void HandleServiceRequest_Finished(bool bResult, object resultContext)
        {
            Debug.WriteLine("Enter Playback.HandleServiceRequest_Finished()");

            Debug.WriteLine("MediaProtectionServiceCompletion.Complete = " + bResult.ToString());
            _serviceCompletionNotifier.Complete(bResult);

            Debug.WriteLine("Leave Playback.HandleServiceRequest_Finished()");
        }

        private void PlayerPage_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.OriginalKey == VirtualKey.GamepadRightTrigger)
            {
                FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                e.Handled = true;
            }
            //if (e.OriginalKey == VirtualKey.GamepadB && Player.Plugins.)
            {
                //e.Handled = true;
            }

            Debug.WriteLine(e.OriginalKey);
        }
    }
}