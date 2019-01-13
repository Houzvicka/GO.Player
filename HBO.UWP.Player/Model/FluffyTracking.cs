using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class FluffyTracking
    {
        [JsonProperty("Filter")]
        public string Filter { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("SortOption")]
        public string SortOption { get; set; }

        [JsonProperty("SubFilter")]
        public string SubFilter { get; set; }
    }
}