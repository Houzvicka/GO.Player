using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Item
    {
        [JsonProperty("ObjectType")]
        public long ObjectType { get; set; }

        [JsonProperty("ObjectUrl")]
        public Uri ObjectUrl { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("BackgroundUrl")]
        public object BackgroundUrl { get; set; }

        [JsonProperty("Color")]
        public string Color { get; set; }

        [JsonProperty("Container")]
        public List<Container> Container { get; set; }

        [JsonProperty("ExpiryMin")]
        public long ExpiryMin { get; set; }

        [JsonProperty("Filters")]
        public List<object> Filters { get; set; }

        [JsonProperty("HighlightedContentNumber")]
        public long HighlightedContentNumber { get; set; }

        [JsonProperty("Host")]
        public string Host { get; set; }

        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("ImageIdentifier")]
        public string ImageIdentifier { get; set; }

        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("IsSelection")]
        public bool IsSelection { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("NormalContentNumber")]
        public long NormalContentNumber { get; set; }

        [JsonProperty("SelectionIndex")]
        public long SelectionIndex { get; set; }

        [JsonProperty("ShowInMenu")]
        public bool ShowInMenu { get; set; }

        [JsonProperty("SortOptions")]
        public List<object> SortOptions { get; set; }

        [JsonProperty("Tracking")]
        public Tracking Tracking { get; set; }

        [JsonProperty("Version")]
        public long Version { get; set; }

        [JsonProperty("ViewType")]
        public long ViewType { get; set; }
    }
}
