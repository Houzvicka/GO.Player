using System;
using GO.UWP.Player.Contracts;

namespace GO.UWP.Player.Services
{
    public class ConfigService : IConfigService
    {
        private string DomainCode = "cz";
        private string CountryCodeShort = "cz";
        private string CountryCode = "CZE";
        private string LanguageCode = "CES";
        private string ApiPlatform = "XBOX";

        private string ApiHost => CountryCodeShort + "api.hbogo.eu";
        private string ApiHostGateway => "https://gateway.hbogo.eu";
        private string ApiHostReferer => "https://gateway.hbogo.eu/signin/form";

        public Uri DefaultOperatorUri => new Uri("https://api.ugw.hbogo.eu/v3.0/Operators/" + CountryCode + "/" + LanguageCode + "/" + ApiPlatform);
        public Uri ListOperatorsUri => new Uri("https://" + CountryCodeShort + "gwapi.hbogo.eu/v2.1/Operators/json/" + LanguageCode + "/" + ApiPlatform);

        public Uri SettingsUri => new Uri("https://" + ApiHost + "/v7/Settings/json/" + LanguageCode + "/" + ApiPlatform);
        public Uri HboAccountLoginUri => new Uri("https://api.ugw.hbogo.eu/v3.0/Authentication/" + CountryCode + "/JSON/" + LanguageCode + "/" + ApiPlatform);
        public Uri OperatorAccountLoginUri => new Uri("https://" + CountryCodeShort + "gwapi.hbogo.eu/v2.1/Authentication/json/" + LanguageCode + "/" + ApiPlatform);
        public Uri SearchUri => new Uri("https://" + ApiHost + "/v7/Search/json/" + LanguageCode + "/" + ApiPlatform + "/");
        public Uri MyCategoryUri => new Uri("https://" + ApiHost + "/v7/CustomerGroup/json/" + LanguageCode + "/" +  ApiPlatform + "/");
        public Uri CategoriesUri => new Uri("https://" + ApiHost + "/v5/Groups/json/" + LanguageCode + "/" + ApiPlatform);
        public Uri NewCategoriesUri => new Uri("https://" + ApiHost + "/v8/Groups/json/" + LanguageCode + "/ANMO/0/True");
        public Uri PlayUri => new Uri("https://" + ApiHost + "/v5/Content/json/" + LanguageCode + "/" + ApiPlatform + "/");
        public Uri PurchaseUri => new Uri("https://" + ApiHost + "/v5/Purchase/Json/" + LanguageCode + "/" + ApiPlatform);
        public Uri LicenceServerUri => new Uri("https://lic.drmtoday.com/license-proxy-widevine/cenc/");

        public Uri AddRating => new Uri("https://" + ApiHost + "/v7/AddRating/json/" + LanguageCode + "/" + ApiPlatform + "/");
        public Uri AddMyList => new Uri("https://" + ApiHost + "/v7/AddWatchlist/json/" + LanguageCode + "/" + ApiPlatform + "/");
        public Uri RemoveMyList => new Uri("https://" + ApiHost + "/v7/RemoveWatchlist/json/" + LanguageCode + "/" + ApiPlatform + "/");


        //public Uri DeviceRegistrationUri => new Uri("https://cz.hbogo.eu/services/settings/silentregister.aspx");
    }
}
