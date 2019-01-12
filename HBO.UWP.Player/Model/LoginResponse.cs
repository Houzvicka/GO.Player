namespace HBO.UWP.Player.Model
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class LoginResponse
    {
        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("Status")]
        public long Status { get; set; }

        [JsonProperty("Customer")]
        public Customer Customer { get; set; }

        [JsonProperty("Error")]
        public Error Error { get; set; }

        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("SessionId")]
        public Guid SessionId { get; set; }
    }
}
