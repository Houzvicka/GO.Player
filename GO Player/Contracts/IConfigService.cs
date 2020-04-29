namespace GO_Player.Services
{
    public interface IConfigService
    {
        /// <summary>
        /// string for PlayReady licence server.
        /// </summary>
        string LicenceServerUri { get; }

        string ApiHost { get; }
        string ApiHostReferer { get; }
        string ApiHostOrigin { get; }
        string ApiHostGateway { get; }
        string ApiHostGatewayReferer { get; }

        string DefaultOperatorUri { get; }
        string ListOperatorsUri { get; }

        /// <summary>
        /// string where device can obtain settings.
        /// </summary>
        string SettingsUri { get; }

        /// <summary>
        /// string for logging direct hbo account.
        /// </summary>
        string HboAccountLoginUri { get; }

        /// <summary>
        /// string for logging operators account.
        /// </summary>
        string OperatorAccountLoginUri { get; }


        /// <summary>
        /// string for customer groups.
        /// </summary>
        string CustomerGroupUri { get; }

        /// <summary>
        /// string for searching.
        /// </summary>
        string MyCategoryUri { get; }

        /// <summary>
        /// string for searching.
        /// </summary>
        string CategoriesUri { get; }

        /// <summary>
        /// string for searching.
        /// </summary>
        string PlayUri { get; }

        /// <summary>
        /// string for searching.
        /// </summary>
        string PurchaseUri { get; }

        /// <summary>
        /// string for searching.
        /// </summary>
        string SearchUri { get; }

        string AddRatingUri { get; }
        string AddMyListUri { get; }
        string RemoveMyListUri { get; }
        string HistoryUri { get; }
    }
}