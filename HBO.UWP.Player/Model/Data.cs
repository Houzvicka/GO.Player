using System;
using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class Data
    {
        [JsonProperty("ValidationResponse")]
        public object ValidationResponse { get; set; }

        [JsonProperty("Customer")]
        public Customer Customer { get; set; }

        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("SessionId")]
        public Guid SessionId { get; set; }

        [JsonProperty("ErrorMessage")]
        public object ErrorMessage { get; set; }

        [JsonProperty("Message")]
        public object Message { get; set; }

        [JsonProperty("NeedDeviceConfirmation")]
        public bool NeedDeviceConfirmation { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }
    }
}