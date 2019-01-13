using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using CommonServiceLocator;
using HBO.UWP.Player.Contracts;
using HBO.UWP.Player.Helpers;
using HBO.UWP.Player.Model;
using Newtonsoft.Json;
using PlayReadyUAP;

namespace HBO.UWP.Player
{
    public sealed partial class MainPage
    {
        private IConfigService config;
        private ISettingsService settings;

        private LoginResponse login;

        public MainPage()
        {
            this.InitializeComponent();

            config = ServiceLocator.Current.GetInstance<IConfigService>();
            settings = ServiceLocator.Current.GetInstance<ISettingsService>();

            AddDefaultHeaders();
            StartupCall();
        }

        HttpClient httpClient = new HttpClient(new HttpBaseProtocolFilter() {AllowAutoRedirect = false});

        public void AddDefaultHeaders()
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134");
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://www.hbogo.cz/");
            httpClient.DefaultRequestHeaders.Add("Origin", "https://www.hbogo.cz");
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            httpClient.DefaultRequestHeaders.Add("GO-Language", "CES");
            httpClient.DefaultRequestHeaders.Add("GO-requiredPlatform", "CHBR");
            httpClient.DefaultRequestHeaders.Add("GO-Token", "");
            httpClient.DefaultRequestHeaders.Add("GO-SessionId", "");
            httpClient.DefaultRequestHeaders.Add("GO-swVersion", "4.8.0");
            httpClient.DefaultRequestHeaders.Add("GO-CustomerId", "");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "");
        }

        public async void StartupCall()
        {
            CurrentDevice dev;
            if (string.IsNullOrEmpty(settings.CurrentDeviceId))
            {
                dev = (await SilentRegister(config.DeviceRegistrationUri)).Data.Customer.CurrentDevice;

                settings.CurrentDeviceId = dev.Id.ToString();
                settings.Individualization = dev.Individualization;
            }
            else
            {
                dev = new CurrentDevice()
                {
                    Id = new Guid(settings.CurrentDeviceId),
                    Individualization = settings.Individualization
                };
            }

            login = await Login(config.HboAccountLoginUri, "houzvickajiri@gmail.com", "primer.magnate.claptrap", 0, dev);
            if (login.Error == null)
            {
                httpClient.DefaultRequestHeaders["GO-SessionId"] = login.SessionId.ToString();
                httpClient.DefaultRequestHeaders["GO-Token"] = login.Token;
                httpClient.DefaultRequestHeaders["GO-CustomerId"] = login.Customer.Id.ToString();
            }

            Categories cats = await GetCategories(config.CategoriesUri);

            mListBox.ItemsSource = cats.Items;
            mListBox.DisplayMemberPath = "Name";
        }

        public async Task<Registration> SilentRegister(Uri registrationUri)
        {
            var response = await httpClient.GetAsync(registrationUri);
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Registration>(await response.Content.ReadAsStringAsync());
        }

        public async Task<LoginResponse> Login(Uri loginUri, string username, string password, int operatorId,
            CurrentDevice device)
        {
            httpClient.DefaultRequestHeaders["Referer"] = "https://gateway.hbogo.eu";
            httpClient.DefaultRequestHeaders["Origin"] = "https://gateway.hbogo.eu";

            Login lg = new Login();

            lg.Action = "L";
            lg.Language = "CES";

            lg.CurrentDevice = device;

            lg.EmailAddress = username;
            lg.Password = password;
            lg.OperatorId = Operators.OperatorsList[operatorId];

            var response = await httpClient.PostAsync(loginUri,
                new HttpStringContent(JsonConvert.SerializeObject(lg), UnicodeEncoding.Utf8, "application/json"));
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Categories> GetCategories(Uri categoriesUri)
        {
            var response = await httpClient.GetAsync(categoriesUri);

            response = await httpClient.GetAsync(new Uri(response.Headers["location"])); //catch redirect

            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Categories>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ContentsItem> GetShowDetail(Uri showUri)
        {
            if (showUri.ToString().Contains("xml"))
            {
                showUri = new Uri(showUri.ToString().Replace("xml", "json"));
            }

            var response = await httpClient.GetAsync(showUri);
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<ContentsItem>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Video> GetPlayableLink(Uri playUri, Guid showGuid)
        {
            var context = new HttpStringContent(
                $"<Purchase xmlns=\"go:v5:interop\"><AllowHighResolution>true</AllowHighResolution><ContentId>{showGuid.ToString()}</ContentId><CustomerId>{httpClient.DefaultRequestHeaders["GO-CustomerId"]}</CustomerId><Individualization>{settings.Individualization}</Individualization><OperatorId>{Operators.OperatorsList[0]}</OperatorId><ClientInfo></ClientInfo><IsFree>false</IsFree><UseInteractivity>false</UseInteractivity></Purchase>");
            var response = await httpClient.PostAsync(playUri, context);
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Video>(await response.Content.ReadAsStringAsync());
        }

        private async void MListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem is CategoriesItem i)
            {
                sListBox.ItemsSource = i.Container.First().Contents.Items;
                sListBox.DisplayMemberPath = "Name";
            }
        }

        private async void SListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem is ContentsItem c)
            {
                tListBox.ItemsSource = (await GetShowDetail(c.ObjectUrl)).ChildContents.Items;
                tListBox.DisplayMemberPath = "Name";
            }
        }

        private async void TListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem is ContentsItem p)
            {
                Video playUrl = await GetPlayableLink(config.PurchaseUri, p.Id);

                Uri mfestUri = new Uri(playUrl.Purchase.MediaUrl.AbsoluteUri + "/manifest");

                //new PRH().InitializeAdaptiveMediaSource(mfestUri, Element);
                Playback plbk = new Playback(Element);
                plbk.RequestConfigData = new ServiceRequestConfigData()
                {
                    Uri = new Uri($"https://lic.drmtoday.com/license-proxy-headerauth/drmtoday/RightsManager.asmx?assetId={playUrl.Purchase.AssetId}&variantId={playUrl.Purchase.VariantId}"),
                    ChallengeCustomData = Base64Helpers.Base64Encode("{\"userId\":\"" + login.Customer.Id + "\",\"sessionId\":\"" + playUrl.Purchase.PlayerSessionId + "\",\"merchant\":\"hboeurope\"}"),
                    CustomArtibutes = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("x-dt-auth-token", playUrl.Purchase.AuthToken),
                        new KeyValuePair<string, string>("Origin", "https://www.hbogo.cz"),
                    }
                };
                plbk.Play(mfestUri.ToString());

                //Element.Source = mfestUri;
                //Element.Play();
            }
        }
    }
}
