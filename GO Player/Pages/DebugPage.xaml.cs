using GO_Player.Models;
using GO_Player.Models.API;
using Newtonsoft.Json;
using System;
using System.Linq;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace GO_Player
{
    public sealed partial class DebugPage : Page
    {
        readonly HttpClient _httpClient = new HttpClient(new HttpBaseProtocolFilter() { AllowAutoRedirect = false });

        public DebugPage()
        {
            this.InitializeComponent();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://www.hbogo.cz/");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://www.hbogo.cz");
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            _httpClient.DefaultRequestHeaders.Add("GO-Language", "CES");
            _httpClient.DefaultRequestHeaders.Add("GO-requiredPlatform", "CHBR");
            _httpClient.DefaultRequestHeaders.Add("GO-Token", "");
            _httpClient.DefaultRequestHeaders.Add("GO-SessionId", "");
            _httpClient.DefaultRequestHeaders.Add("GO-swVersion", "4.8.0");
            _httpClient.DefaultRequestHeaders.Add("GO-CustomerId", "");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

            _httpClient.DefaultRequestHeaders["Origin"] = "https://gateway.hbogo.eu";
            _httpClient.DefaultRequestHeaders["Referer"] = "https://gateway.hbogo.eu/signin/form";
        }
        
        private Item curroperator = new Item();
        private Region selectedCountry = Constants.Countries.First(c => c.TDomain == "cz");

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var response = await _httpClient.GetAsync(new Uri(string.Format("https://api.ugw.hbogo.eu/v3.0/Operators/{0}/JSON/{1}/{2}", selectedCountry.CountryCode, selectedCountry.LanguageCode, "XONE")));
            Operators strresp = JsonConvert.DeserializeObject<Operators>(await response.Content.ReadAsStringAsync());
            curroperator = strresp.Items.First();
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            Guid CurrentDeviceId = Guid.NewGuid();
            Guid Individualization = Guid.NewGuid();

            Login lg = new Login();

            lg.Action = "L";
            lg.Language = "CES";

            var clientDeviceInformation = new EasClientDeviceInformation();
            string friendlyName = clientDeviceInformation.FriendlyName;

            lg.CurrentDevice = new  CurrentDevice() { Individualization = Individualization, Id = CurrentDeviceId, Name = friendlyName };

            lg.EmailAddress = "houzvickajiri@gmail.com";
            lg.Password = "primer.magnate.claptrap";
            lg.OperatorId = Guid.Empty;

            var jsonRequest = JsonConvert.SerializeObject(lg);
            var requestContent = new HttpStringContent(jsonRequest, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
            var response = await _httpClient.PostAsync(new Uri(string.Format("https://api.ugw.hbogo.eu/v3.0/Authentication/{0}/JSON/{1}/{2}/", selectedCountry.CountryCode, selectedCountry.LanguageCode, "XONE")), requestContent);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
