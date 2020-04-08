﻿using System;

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
        Uri HboAccountLoginUri { get; }

        /// <summary>
        /// Uri for logging operators account.
        /// </summary>
        Uri OperatorAccountLoginUri { get; }

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri SearchUri { get; }

        /// <summary>
        /// Uri for loading favorites and other preferences.
        /// </summary>
        Uri SettingsUri { get; }

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri MyCategoryUri { get; }

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri CategoriesUri { get; }

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri NewCategoriesUri { get; }

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri PlayUri { get; }

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri LicenceServerUri { get; }

        /// <summary>
        /// Uri for searching.
        /// </summary>
        Uri PurchaseUri { get; }
    }
}
