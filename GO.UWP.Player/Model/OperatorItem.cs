using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class OperatorItem
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("IsPromoted")]
        public bool IsPromoted { get; set; }

        [JsonProperty("PopupHeight")]
        public long PopupHeight { get; set; }

        [JsonProperty("LiveStreamSupport")]
        public bool LiveStreamSupport { get; set; }

        [JsonProperty("DownloadSupport")]
        public bool DownloadSupport { get; set; }

        [JsonProperty("DefaultCommercialStatus")]
        public long DefaultCommercialStatus { get; set; }

        [JsonProperty("AuthenticationType")]
        public long AuthenticationType { get; set; }

        [JsonProperty("IsOwn")]
        public long IsOwn { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("RegistrationPageDisabled")]
        public bool RegistrationPageDisabled { get; set; }

        [JsonProperty("WeightIndex")]
        public long WeightIndex { get; set; }

        [JsonProperty("HideFromRegistration")]
        public bool HideFromRegistration { get; set; }

        [JsonProperty("Parameters")]
        public Parameter[] Parameters { get; set; }

        [JsonProperty("Templates")]
        public object[] Templates { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("HelpTextOperator", NullValueHandling = NullValueHandling.Ignore)]
        public string HelpTextOperator { get; set; }

        [JsonProperty("LogoUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri LogoUrl { get; set; }

        [JsonProperty("WebsiteUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string WebsiteUrl { get; set; }
    }
}
