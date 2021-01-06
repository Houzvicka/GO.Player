using System;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using GO.UWP.Player.Contracts;
using GO.UWP.Player.Model;
using Newtonsoft.Json;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace GO.UWP.Player.Services
{
    public class CommunicationService : ICommunicationService
    {
        readonly HttpClient _httpClient = new HttpClient(new HttpBaseProtocolFilter() { AllowAutoRedirect = false });

        public CommunicationService()
        {
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
        }

        public async Task<Registration> SilentRegister(Uri registrationUri)
        {
            var response = await _httpClient.GetAsync(registrationUri);
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Registration>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Operators> GetOperators(Uri defaultOperatorUri)
        {
            var response = await _httpClient.GetAsync(defaultOperatorUri);
            string stringResponse = await response.Content.ReadAsStringAsync();
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Operators>(stringResponse);
        }

        public async Task<LoginResponse> Login(Uri loginUri, string username, string password, Guid operatorId,
            CurrentDevice device)
        {
            try
            {
                _httpClient.DefaultRequestHeaders["Origin"] = "https://gateway.hbogo.eu";
                _httpClient.DefaultRequestHeaders["Referer"] = "https://gateway.hbogo.eu/signin/form";

                Login lg = new Login();

                lg.Action = "L";
                lg.Language = "CES";

                lg.CurrentDevice = device;

                EasClientDeviceInformation eas = new EasClientDeviceInformation();

                //TEST
                lg.BirthYear = 1;
                if(lg.CurrentDevice == null) lg.CurrentDevice = new CurrentDevice();
                lg.CurrentDevice.Brand = eas.SystemManufacturer;
                lg.CurrentDevice.Modell = eas.SystemProductName;
                lg.CurrentDevice.OsName = "Windows";
                lg.CurrentDevice.OsVersion = eas.SystemFirmwareVersion;
                lg.CurrentDevice.Platform = "XONE";
                lg.CurrentDevice.SwVersion = "3.3.9.6418.2100";
                //

                lg.EmailAddress = username;
                lg.Password = password;
                lg.OperatorId = operatorId;

                var jsonRequest = JsonConvert.SerializeObject(lg);
                var requestContent = new HttpStringContent(jsonRequest, UnicodeEncoding.Utf8, "application/json");
                var response = await _httpClient.PostAsync(loginUri, requestContent);

                var login = !response.IsSuccessStatusCode
                    ? null
                    : JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());

                if (login != null && login.Error == null)
                {
                    _httpClient.DefaultRequestHeaders["GO-SessionId"] = login.SessionId.ToString();
                    _httpClient.DefaultRequestHeaders["GO-Token"] = login.Token;
                    _httpClient.DefaultRequestHeaders["GO-CustomerId"] = login.Customer.Id.ToString();
                }

                return login;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return null;
            }
        }

        public async Task<CategoriesItem> GetCategory(Uri categoriesUri)
        {
            var response = await _httpClient.GetAsync(categoriesUri);

            if(!response.IsSuccessStatusCode) response = await _httpClient.GetAsync(new Uri(response.Headers["location"])); //catch redirect

            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<CategoriesItem>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Categories> GetCategories(Uri categoriesUri)
        {
            var response = await _httpClient.GetAsync(categoriesUri);

            if (!response.IsSuccessStatusCode) response = await _httpClient.GetAsync(new Uri(response.Headers["location"])); //catch redirect

            var stringResponse = await response.Content.ReadAsStringAsync();

            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Categories>(stringResponse);
        }

        public async Task<ContentsItem> GetShowDetail(Uri showUri)
        {
            if (showUri.ToString().Contains("xml"))
            {
                showUri = new Uri(showUri.ToString().Replace("xml", "json"));
            }

            var response = await _httpClient.GetAsync(showUri);
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<ContentsItem>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Video> GetPlayableLink(Uri playUri, Guid showGuid, string individualization, Guid operatorGuid, string languageCode, string apiPlatform)
        {
            var context = new HttpStringContent(
                $"<Purchase xmlns=\"go:v8:interop\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><AirPlayAllowed>false</AirPlayAllowed><AllowHighResolution>true </AllowHighResolution><ContentId>{showGuid.ToString()}</ContentId><CustomerId>{_httpClient.DefaultRequestHeaders["GO-CustomerId"]}</CustomerId><Individualization>{individualization}</Individualization><OperatorId>{operatorGuid}</OperatorId><ApplicationLanguage>{languageCode}' + self.LANGUAGE_CODE + '</ApplicationLanguage><IsFree>false</IsFree><PreferedAudio>{languageCode}' + self.LANGUAGE_CODE + '</PreferedAudio><PreferedSubtitle>{languageCode}</PreferedSubtitle><PreferredAudioType>Stereo</PreferredAudioType><RequiredPlatform>{apiPlatform}</RequiredPlatform><UseInteractivity>false</UseInteractivity></Purchase>");
            var response = await _httpClient.PostAsync(playUri, context);
            var stringResponse = await response.Content.ReadAsStringAsync();
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Video>(stringResponse);
        }

        public async Task<Item> GetSearchResults(Uri searchUri, string searchQuery)
        {
            var response = await _httpClient.GetAsync(new Uri(searchUri + searchQuery + "/0"));

            response = await _httpClient.GetAsync(new Uri(response.Headers["location"])); //catch redirect

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Item>(await response.Content.ReadAsStringAsync());
        }
    }
}
