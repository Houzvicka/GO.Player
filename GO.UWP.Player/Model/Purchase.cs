using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
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
}