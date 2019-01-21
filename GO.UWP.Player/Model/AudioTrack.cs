using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class AudioTrack
    {
        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Url")]
        public object Url { get; set; }
    }
}