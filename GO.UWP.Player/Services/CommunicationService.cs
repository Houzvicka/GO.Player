using System;
using System.Threading.Tasks;
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
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
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
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "");
        }

        public async Task<Registration> SilentRegister(Uri registrationUri)
        {
            var response = await _httpClient.GetAsync(registrationUri);
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Registration>(await response.Content.ReadAsStringAsync());
        }

        public async Task<LoginResponse> Login(Uri loginUri, string username, string password, int operatorId,
            CurrentDevice device)
        {
            try
            {
                _httpClient.DefaultRequestHeaders["Referer"] = "https://gateway.hbogo.eu";
                _httpClient.DefaultRequestHeaders["Origin"] = "https://gateway.hbogo.eu";

                Login lg = new Login();

                lg.Action = "L";
                lg.Language = "CES";

                lg.CurrentDevice = device;

                lg.EmailAddress = username;
                lg.Password = password;
                lg.OperatorId = Operators.OperatorsList[operatorId];

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

            var response = await _httpClient.GetAsync(showUri);
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<ContentsItem>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Video> GetPlayableLink(Uri playUri, Guid showGuid, string individualization)
        {
            var context = new HttpStringContent(
                $"<Purchase xmlns=\"go:v5:interop\"><AllowHighResolution>true</AllowHighResolution><ContentId>{showGuid.ToString()}</ContentId><CustomerId>{_httpClient.DefaultRequestHeaders["GO-CustomerId"]}</CustomerId><Individualization>{individualization}</Individualization><OperatorId>{Operators.OperatorsList[0]}</OperatorId><ClientInfo></ClientInfo><IsFree>false</IsFree><UseInteractivity>false</UseInteractivity></Purchase>");
            var response = await _httpClient.PostAsync(playUri, context);
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Video>(await response.Content.ReadAsStringAsync());
        }

        public async Task<SearchResult> GetSearchResults(Uri searchUri, string searchQuery)
        {
            var response = await _httpClient.GetAsync(new Uri(searchUri + searchQuery + "/0"));

            response = await _httpClient.GetAsync(new Uri(response.Headers["location"])); //catch redirect

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<SearchResult>(await response.Content.ReadAsStringAsync());
        }
    }
}
