using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class Video
    {
        [JsonProperty("Host")]
        public string Host { get; set; }

        [JsonProperty("Purchase")]
        public Purchase Purchase { get; set; }

        [JsonProperty("ResponseStatus")]
        public long ResponseStatus { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Tracking")]
        public Tracking Tracking { get; set; }
    }
}
