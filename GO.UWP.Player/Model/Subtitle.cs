using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Subtitle
    {
        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("IsDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Type")]
        public long Type { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }
    }
}