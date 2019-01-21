using System.Collections.Generic;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Categories
    {
        [JsonProperty("Host")]
        public string Host { get; set; }

        [JsonProperty("Items")]
        public List<CategoriesItem> Items { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }
    }
}
