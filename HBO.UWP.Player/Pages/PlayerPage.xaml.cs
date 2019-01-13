using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HBO.UWP.Player.Helpers;
using HBO.UWP.Player.Helpers.Playback;
using HBO.UWP.Player.Model;

namespace HBO.UWP.Player.Pages
{
    public sealed partial class PlayerPage : Page
    {
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

        public void Play(Uri mediaUri)
        {
            plbk.Play(mediaUri);
        }
    }
}
