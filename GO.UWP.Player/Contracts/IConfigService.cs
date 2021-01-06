using System;

namespace GO.UWP.Player.Contracts
{
    public interface IConfigService
    {
        /// <summary>
        /// Uri where device can obtain registration.
        /// </summary>
        //Uri DeviceRegistrationUri { get; }

        /// <summary>
        /// Uri for logging direct hbo account.
        /// </summary>
        Uri HboAccountLoginUri(string countryCode, string languageCode, string apiPlatform);

        /// <summary>
        /// Uri for logging operators account.
        /// </summary>
        Uri OperatorAccountLoginUri(string countryCodeShort, string languageCode, string apiPlatform);

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri SearchUri(string countryCodeShort, string languageCode, string apiPlatform);

        /// <summary>
        /// Uri for getting default operator value.
        /// </summary>
        Uri DefaultOperatorUri(string countryCode, string languageCode, string apiPlatform);

        /// <summary>
        /// Uri for getting other operator value.
        /// </summary>
        Uri ListOperatorsUri(string countryCodeShort, string languageCode, string apiPlatform);

        /// <summary>
        /// Uri for loading favorites and other preferences.
        /// </summary>
        Uri SettingsUri(string countryCodeShort, string languageCode, string apiPlatform);

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri MyCategoryUri(string countryCodeShort, string languageCode, string apiPlatform);

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri CategoriesUri(string countryCodeShort, string languageCode);

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri PlayUri(string countryCodeShort, string languageCode, string apiPlatform);

        /// <summary>
        /// Uri for searching.
        /// </summary>
        //Uri LicenceServerUri { get; }

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri PurchaseUri(string countryCodeShort, string languageCode, string apiPlatform);
    }
}
