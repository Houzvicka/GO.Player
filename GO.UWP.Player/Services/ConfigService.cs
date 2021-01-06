using System;
using System.Collections.Generic;
using GO.UWP.Player.Contracts;

namespace GO.UWP.Player.Services
{
    public class ConfigService : IConfigService
    {
        private string ApiHost(string countryCodeShort) => countryCodeShort + "api.hbogo.eu";

        private string ApiHostReferer(string domainCode) => "https://hbogo." + domainCode + "/";
        private string ApiHostOrigin(string domainCode) => "https://www.hbogo." + domainCode;

        private string ApiHostGateway => "https://gateway.hbogo.eu";
        private string ApiHostGatewayReferer => "https://gateway.hbogo.eu/signin/form";

        public Uri DefaultOperatorUri(string countryCode, string languageCode, string apiPlatform) => new Uri("https://api.ugw.hbogo.eu/v3.0/Operators/" + countryCode + "/JSON/" + languageCode + "/" + apiPlatform);
        public Uri ListOperatorsUri(string countryCodeShort, string languageCode, string apiPlatform) => new Uri("https://" + countryCodeShort + "gwapi.hbogo.eu/v2.1/Operators/json/" + languageCode + "/" + apiPlatform);

        public Uri SettingsUri(string countryCodeShort, string languageCode, string apiPlatform) => new Uri("https://" + ApiHost(countryCodeShort) + "/v8/Settings/json/" + languageCode + "/" + apiPlatform);
        public Uri HboAccountLoginUri(string countryCode, string languageCode, string apiPlatform) => new Uri("https://api.ugw.hbogo.eu/v3.0/Authentication/" + countryCode + "/JSON/" + languageCode + "/" + apiPlatform);
        public Uri OperatorAccountLoginUri(string countryCodeShort, string languageCode, string apiPlatform) => new Uri("https://" + countryCodeShort + "gwapi.hbogo.eu/v2.1/Authentication/json/" + languageCode + "/" + apiPlatform);
        public Uri SearchUri(string countryCodeShort, string languageCode, string apiPlatform) => new Uri("https://" + ApiHost(countryCodeShort) + "/v8/Search/json/" + languageCode + "/" + apiPlatform + "/");
        public Uri MyCategoryUri(string countryCodeShort, string languageCode, string apiPlatform) => new Uri("https://" + ApiHost(countryCodeShort) + "/v8/CustomerGroup/json/" + languageCode + "/" + apiPlatform + "/");
        public Uri CategorieUri(string countryCodeShort, string languageCode) => new Uri("https://" + ApiHost(countryCodeShort) + "/v8/Group/json/" + languageCode + "/ANMO/");
        public Uri CategoriesUri(string countryCodeShort, string languageCode) => new Uri("https://" + ApiHost(countryCodeShort) + "/v8/Groups/json/" + languageCode + "/ANMO/0/True");
        public Uri PlayUri(string countryCodeShort, string languageCode, string apiPlatform) => new Uri("https://" + ApiHost(countryCodeShort) + "/v8/Content/json/" + languageCode + "/" + apiPlatform + "/");
        public Uri PurchaseUri(string countryCodeShort, string languageCode, string apiPlatform) => new Uri("https://" + ApiHost(countryCodeShort) + "/v8/Purchase/Json/" + languageCode + "/" + apiPlatform);

        //public Uri LicenceServerUri => new Uri("https://lic.drmtoday.com/license-proxy-headerauth/drmtoday/RightsManager.asmx?assetId={purchase.AssetId}&variantId={purchase.VariantId}");

        //public Uri AddRatingUri => new Uri("https://" + ApiHost + "/v8/AddRating/json/" + LanguageCode + "/" + ApiPlatform + "/");
        //public Uri AddMyListUri => new Uri("https://" + ApiHost + "/v8/AddWatchlist/json/" + LanguageCode + "/" + ApiPlatform + "/");
        //public Uri RemoveMyListUri => new Uri("https://" + ApiHost + "/v8/RemoveWatchlist/json/" + LanguageCode + "/" + ApiPlatform + "/");

        public Uri HistoryUri => new Uri("https://bookmarking.hbogo.eu/v1/History/");

        //public Uri DeviceRegistrationUri => new Uri("https://cz.hbogo.eu/services/settings/silentregister.aspx");
        }
}
