using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using HBO.UWP.Player.Contracts;
using HBO.UWP.Player.Model;
using Newtonsoft.Json;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace HBO.UWP.Player.Services
{
    public class CommunicationService : ICommunicationService
    {
        HttpClient httpClient = new HttpClient(new HttpBaseProtocolFilter() { AllowAutoRedirect = false });

        public CommunicationService()
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

            var login = !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());

            if (login != null && login.Error == null)
            {
                httpClient.DefaultRequestHeaders["GO-SessionId"] = login.SessionId.ToString();
                httpClient.DefaultRequestHeaders["GO-Token"] = login.Token;
                httpClient.DefaultRequestHeaders["GO-CustomerId"] = login.Customer.Id.ToString();
            }

            return login;
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

        public async Task<Video> GetPlayableLink(Uri playUri, Guid showGuid, string individualization)
        {
            var context = new HttpStringContent(
                $"<Purchase xmlns=\"go:v5:interop\"><AllowHighResolution>true</AllowHighResolution><ContentId>{showGuid.ToString()}</ContentId><CustomerId>{httpClient.DefaultRequestHeaders["GO-CustomerId"]}</CustomerId><Individualization>{individualization}</Individualization><OperatorId>{Operators.OperatorsList[0]}</OperatorId><ClientInfo></ClientInfo><IsFree>false</IsFree><UseInteractivity>false</UseInteractivity></Purchase>");
            var response = await httpClient.PostAsync(playUri, context);
            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<Video>(await response.Content.ReadAsStringAsync());
        }
    }
}
