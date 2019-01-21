using System;
using GO.UWP.Player.Contracts;

namespace GO.UWP.Player.Services
{
    public class ConfigService : IConfigService
    {
        public Uri DeviceRegistrationUri => new Uri("https://cz.hbogo.eu/services/settings/silentregister.aspx");
        public Uri HboAccountLoginUri => new Uri("https://api.ugw.hbogo.eu/v3.0/Authentication/CES/JSON/CES/COMP");
        public Uri OtherAccountLoginUri => new Uri("https://czgwapi.hbogo.eu/v2.1/Authentication/json/CES/COMP");
        public Uri SearchUri => new Uri("https://czapi.hbogo.eu/v7/Search/json/CES/COMP/");
        public Uri FavoritesUri => new Uri("https://czapi.hbogo.eu/v7/Settings/json/CES/COMP");
        public Uri MyCategoryUri => new Uri("https://czapi.hbogo.eu/v7/CustomerGroup/json/CES/COMP/");
        public Uri CategoriesUri => new Uri("https://czapi.hbogo.eu/v5/Groups/json/CES/COMP");
        public Uri PlayUri => new Uri("http://czapi.hbogo.eu/v5/Content/json/CES/COMP/");
            // http://czapi.hbogo.eu/player50.svc/Content/json/CES/COMP/
            // http://czapi.hbogo.eu/player50.svc/Content/json/CES/APPLE/
            // http://czapi.hbogo.eu/player50.svc/Content/json/CES/SONY/
        public Uri LicenceServerUri => new Uri("https://lic.drmtoday.com/license-proxy-widevine/cenc/");
        //public Uri PurchaseUri => new Uri("https://czapi.hbogo.eu/v5/Purchase/Json/CES/COMP");
        public Uri PurchaseUri => new Uri("https://czapi.hbogo.eu/v5/Purchase/Json/CES/COMP");
    }
}
