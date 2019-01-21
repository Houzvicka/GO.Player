using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Registration
    {
        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Data")]
        public Data Data { get; set; }
    }
}