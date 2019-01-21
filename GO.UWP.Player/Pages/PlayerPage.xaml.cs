using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GO.UWP.Player.Helpers;
using GO.UWP.Player.Helpers.Playback;
using GO.UWP.Player.Model;
using GO.UWP.Player.ViewModel;

namespace GO.UWP.Player.Pages
{
    public sealed partial class PlayerPage : Page
    {
        private MainViewModel mvm => (MainViewModel)DataContext;

        private Playback plbk;
        
        public PlayerPage()
        {
            this.InitializeComponent();

            plbk = new Playback(Element);
        }

        public void SetupRequestConfigData(Guid customerId, Purchase purchase)
        {
            plbk.RequestConfigData = new ServiceRequestConfigData()
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

            Stop();
        }

        public void Play(Video currVid)
        {
            SetupRequestConfigData(mvm.CurrentUser.Customer.Id, currVid.Purchase);
            var playUri = new Uri(currVid.Purchase.MediaUrl.AbsoluteUri + "/manifest");
            plbk.Play(playUri);
        }

        public void Stop()
        {
            plbk.Stop();
        }
    }
}
