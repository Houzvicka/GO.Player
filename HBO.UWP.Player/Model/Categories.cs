using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBO.UWP.Player.Model
{
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

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
