using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Protection;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GO.UWP.Player.Helpers;
using GO.UWP.Player.Helpers.Playback;
using GO.UWP.Player.Model;
using GO.UWP.Player.ViewModel;
using AudioTrack = GO.UWP.Player.Model.AudioTrack;

namespace GO.UWP.Player.Pages
{
    public sealed partial class PlayerPage : Page
    {
        private MainViewModel mvm => (MainViewModel)DataContext;

        //private Playback plbk;
        
        public PlayerPage()
        {
            this.InitializeComponent();

            //plbk = new Playback(Element);

            var player = new MediaPlayer();

            Element2.SetMediaPlayer(player);
            SetupProtectionManager(Element2.MediaPlayer);

            
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
            
            Play(mvm.CurrentlySelectedVideo);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            Pause();
        }

        public void Play(Video currVid)
        {
            SetupRequestConfigData(mvm.CurrentUser.Customer.Id, currVid.Purchase);
            var playUri = new Uri(currVid.Purchase.MediaUrl.AbsoluteUri + "/manifest");
            //plbk.LoadMediaToSource(Element, playUri, true);

            var media = MediaSource.CreateFromUri(playUri);

            foreach (var sub in currVid.Purchase.Subtitles)
            {
                var tts = TimedTextSource.CreateFromUri(new Uri(sub.Url), sub.Name);
                tts.Resolved += Tts_Resolved;

                media.ExternalTimedTextSources.Add(tts);
            }

            var playbackItem = new MediaPlaybackItem(media);

            // Present the first track
            playbackItem.TimedMetadataTracksChanged += (sender, args) =>
            {
                playbackItem.TimedMetadataTracks.SetPresentationMode(0, TimedMetadataTrackPresentationMode.PlatformPresented);
            };

            Element2.MediaPlayer.Source = playbackItem;
            Element2.MediaPlayer.Play();
        }

        private void Tts_Resolved(TimedTextSource sender, TimedTextSourceResolveResultEventArgs args)
        {
            // Handle errors
            if (args.Error != null)
            {
                Debug.WriteLine(args.Error);
                return;
            }
        }

        public void Pause()
        {
            Element2.MediaPlayer.Pause();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (sender is ListBox lb && lb.SelectedItem is AudioTrack at) SetAudioLanguage(at.Code, Element);
        }

        private bool SetAudioLanguage(string lcid, MediaElement media)
        {
            bool wasLanguageSet = false;

            for (int index = 0; index < media.AudioStreamCount; index++)
            {
                if (media.GetAudioStreamLanguage(index) == lcid)
                {
                    media.AudioStreamIndex = index;
                    wasLanguageSet = true;
                }
            }

            return wasLanguageSet;
        }

        private MediaProtectionManager _protectionManager = null;
        Windows.Media.MediaExtensionManager _extensions = null;

        void SetupProtectionManager(MediaPlayer mediaElement)
        {
            Debug.WriteLine("Enter Playback.SetupProtectionManager()");

            Debug.WriteLine("Creating protection system mappings...");
            _protectionManager = new MediaProtectionManager();

            _protectionManager.ComponentLoadFailed += new ComponentLoadFailedEventHandler(ProtectionManager_ComponentLoadFailed);
            _protectionManager.ServiceRequested += new ServiceRequestedEventHandler(ProtectionManager_ServiceRequested);

            Debug.WriteLine("Creating protection system mappings...");
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

            Debug.WriteLine("Creating media extension manager...");
            _extensions = new Windows.Media.MediaExtensionManager();

            Debug.WriteLine("Registering ByteStreamHandlers for PIFF content");
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
                int UseSoftwareProtectionLayer = (int)localSettings.Containers["PlayReady"].Values["SoftwareOverride"];

                if (UseSoftwareProtectionLayer == 1)
                {
                    Debug.WriteLine(" ");
                    Debug.WriteLine("***** Use Software Protection Layer ******");
                    _protectionManager.Properties.Add("Windows.Media.Protection.UseSoftwareProtectionLayer", true);
                }
            }

            mediaElement.ProtectionManager = _protectionManager;

            Debug.WriteLine("Leave Playback.SetProtectionManager()");
        }

        void ProtectionManager_ComponentLoadFailed(MediaProtectionManager sender, ComponentLoadFailedEventArgs e)
        {
            Debug.WriteLine("Enter Playback.ProtectionManager_ComponentLoadFailed()");
            Debug.WriteLine(e.Information.ToString());

            //  List the failing components - RevocationAndRenewalInformation
            for (int i = 0; i < e.Information.Items.Count; i++)
            {
                Debug.WriteLine(e.Information.Items[i].Name + "\nReasons=0x" + e.Information.Items[i].Reasons + "\n"
                                + "Renewal Id=" + e.Information.Items[i].RenewalId);

            }
            e.Completion.Complete(false);
            Debug.WriteLine("Leave Playback.ProtectionManager_ComponentLoadFailed()");
        }

        public ServiceRequestConfigData RequestConfigData
        {
            set { this._requestConfigData = value; }
            get { return this._requestConfigData; }
        }

        RequestChain _requestChain = null;
        ServiceRequestConfigData _requestConfigData = null;
        MediaProtectionServiceCompletion _serviceCompletionNotifier = null;

        void ProtectionManager_ServiceRequested(MediaProtectionManager sender, ServiceRequestedEventArgs srEvent)
        {
            Debug.WriteLine("Enter Playback.ProtectionManager_ServiceRequested()");

            _serviceCompletionNotifier = srEvent.Completion;
            IPlayReadyServiceRequest serviceRequest = (IPlayReadyServiceRequest)srEvent.Request;
            Debug.WriteLine("Servie request type = " + serviceRequest.GetType());

            _requestChain = new RequestChain(serviceRequest);
            _requestChain.RequestConfigData = this.RequestConfigData;
            _requestChain.FinishAndReportResult(new ReportResultDelegate(HandleServiceRequest_Finished));

            Debug.WriteLine("Leave Playback.ProtectionManager_ServiceRequested()");
        }

        void HandleServiceRequest_Finished(bool bResult, object resultContext)
        {
            Debug.WriteLine("Enter Playback.HandleServiceRequest_Finished()");

            Debug.WriteLine("MediaProtectionServiceCompletion.Complete = " + bResult.ToString());
            _serviceCompletionNotifier.Complete(bResult);

            Debug.WriteLine("Leave Playback.HandleServiceRequest_Finished()");
        }
    }
}
