using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class Error
    {
        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("DisplayMessage")]
        public string DisplayMessage { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }
    }
}