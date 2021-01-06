using System.Collections.Generic;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Operators
    {
        [JsonProperty("Host")]
        public string Host { get; set; }

        [JsonProperty("Items")]
        public List<OperatorItem> Items { get; set; }
    }
}