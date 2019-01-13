using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class PurpleTracking
    {
        [JsonProperty("AssetName")]
        public string AssetName { get; set; }

        [JsonProperty("EpisodeName")]
        public string EpisodeName { get; set; }

        [JsonProperty("EpisodeNumber")]
        public long EpisodeNumber { get; set; }

        [JsonProperty("ExternalId")]
        public string ExternalId { get; set; }

        [JsonProperty("Genre")]
        public string Genre { get; set; }

        [JsonProperty("IsLive")]
        public bool IsLive { get; set; }

        [JsonProperty("IsTrailer")]
        public bool IsTrailer { get; set; }

        [JsonProperty("SeasonNumber")]
        public long SeasonNumber { get; set; }

        [JsonProperty("SecondaryGenre")]
        public string SecondaryGenre { get; set; }

        [JsonProperty("ShowName")]
        public string ShowName { get; set; }
    }
}