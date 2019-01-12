using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class Video
    {
        [JsonProperty("Host")]
        public string Host { get; set; }

        [JsonProperty("Purchase")]
        public Purchase Purchase { get; set; }

        [JsonProperty("ResponseStatus")]
        public long ResponseStatus { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Tracking")]
        public Tracking Tracking { get; set; }
    }

    public partial class Purchase
    {
        [JsonProperty("AssetId")]
        public string AssetId { get; set; }

        [JsonProperty("AuthToken")]
        public string AuthToken { get; set; }

        [JsonProperty("AuthXml")]
        public string AuthXml { get; set; }

        [JsonProperty("CDNInfo")]
        public string CdnInfo { get; set; }

        [JsonProperty("ContentKey")]
        public string ContentKey { get; set; }

        [JsonProperty("LicenseUrl")]
        public object LicenseUrl { get; set; }

        [JsonProperty("MediaUrl")]
        public Uri MediaUrl { get; set; }

        [JsonProperty("Merchant")]
        public string Merchant { get; set; }

        [JsonProperty("PlayerSessionId")]
        public Guid PlayerSessionId { get; set; }

        [JsonProperty("VariantId")]
        public string VariantId { get; set; }

        [JsonProperty("AudioTracks")]
        public List<AudioTrack> AudioTracks { get; set; }

        [JsonProperty("Subtitles")]
        public List<Subtitle> Subtitles { get; set; }

        [JsonProperty("AdvertisementSegments")]
        public List<object> AdvertisementSegments { get; set; }

        [JsonProperty("AllowAirPlay")]
        public bool AllowAirPlay { get; set; }

        [JsonProperty("CreditRollStart")]
        public long CreditRollStart { get; set; }

        [JsonProperty("HighResolutionMediaUrl")]
        public object HighResolutionMediaUrl { get; set; }

        [JsonProperty("ResponsePlatform")]
        public long ResponsePlatform { get; set; }

        [JsonProperty("SecondaryMediaUrl")]
        public Uri SecondaryMediaUrl { get; set; }

        [JsonProperty("StartPosition")]
        public long StartPosition { get; set; }
    }

    public partial class Tracking
    {
        [JsonProperty("CDN")]
        public string Cdn { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("Host")]
        public string Host { get; set; }

        [JsonProperty("Operator")]
        public string Operator { get; set; }

        [JsonProperty("StreamUrl")]
        public Uri StreamUrl { get; set; }
    }
}
