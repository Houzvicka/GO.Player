using System;
using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class ParentalControl
    {
        [JsonProperty("Active")]
        public bool Active { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Rating")]
        public long Rating { get; set; }

        [JsonProperty("ReferenceId")]
        public Guid ReferenceId { get; set; }
    }
}