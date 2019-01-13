using System.Collections.Generic;
using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class ChildContents
    {
        [JsonProperty("Items")]
        public List<ContentsItem> Items { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }
    }
}