using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class SortOption
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("IsSelected")]
        public bool IsSelected { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}