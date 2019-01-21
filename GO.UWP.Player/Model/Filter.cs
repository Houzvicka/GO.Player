using System.Collections.Generic;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Filter
    {
        [JsonProperty("Filters")]
        public List<object> Filters { get; set; }

        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("IsSelected")]
        public bool IsSelected { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("ViewType")]
        public long ViewType { get; set; }
    }
}