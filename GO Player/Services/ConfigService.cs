using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO_Player.Services
{
    public class ConfigService : IConfigService
    {
        public string LicenceServerUri => "https://lic.drmtoday.com/license-proxy-headerauth/drmtoday/RightsManager.asmx?assetId={0}&variantId={1}"; //purchase.AssetId, purchase.VariantId

        public string ApiHost => "{0}api.hbogo.eu"; //CountryCodeShort
        public string ApiHostReferer => "https://hbogo.{0}/"; //DomainCode
        public string ApiHostOrigin => "https://www.hbogo.{0}"; //DomainCode
        public string ApiHostGateway => "https://gateway.hbogo.eu";
        public string ApiHostGatewayReferer => "https://gateway.hbogo.eu/signin/form";

        public string DefaultOperatorUri => "https://api.ugw.hbogo.eu/v3.0/Operators/{0}/JSON/{1}/{2}"; //CountryCode, LanguageCode, ApiPlatform
        public string ListOperatorsUri => "https://{0}gwapi.hbogo.eu/v2.1/Operators/json/{1}/{2}"; //CountryCodeShort, LanguageCode, ApiPlatform

        public string SettingsUri => "https://{0}/v8/Settings/json/{1}/ANMO"; //ApiHost, LanguageCode
        public string HboAccountLoginUri => "https://api.ugw.hbogo.eu/v3.0/Authentication/{0}/JSON/{1}/{2}/"; //CountryCode, LanguageCode, ApiPlatform
        public string OperatorAccountLoginUri => "https://{0}gwapi.hbogo.eu/v2.1/Authentication/json/{1}/{2}/"; //CountryCodeShort, LanguageCode, ApiPlatform

        public string CustomerGroupUri => "https://{0}/v8/CustomerGroup/json/{1}/{2}/"; //ApiHost, LanguageCode, ApiPlatform
        public string MyCategoryUri => "https://{0}/v8/Group/json/{1}/ANMO/"; //ApiHost, LanguageCode
        public string CategoriesUri => "https://{0}/v8/Groups/json/{1}/ANMO/0/True"; //ApiHost, LanguageCode
        public string PlayUri => "https://{0}/v8/Content/json/{1}/{2}/"; //ApiHost, LanguageCode, ApiPlatform
        public string PurchaseUri => "https://{0}/v8/Purchase/Json/{1}/{2}/"; //ApiHost, LanguageCode, ApiPlatform
        public string SearchUri => "https://{0}/v8/Search/json/{1}/{2}/"; //ApiHost, LanguageCode, ApiPlatform

        public string AddRatingUri => "https://{0}/v8/AddRating/json/{1}/{2}/"; //ApiHost, LanguageCode, ApiPlatform
        public string AddMyListUri => "https://{0}/v8/AddWatchlist/json/{1}/{2}/"; //ApiHost, LanguageCode, ApiPlatform
        public string RemoveMyListUri => "https://{0}/v8/RemoveWatchlist/json/{1}/{2}/"; //ApiHost, LanguageCode, ApiPlatform
        public string HistoryUri => "https://bookmarking.hbogo.eu/v1/History/";
    }
}
