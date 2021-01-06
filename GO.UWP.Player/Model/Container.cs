using System;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Container
    {
        [JsonProperty("ObjectType")]
        public long ObjectType { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Contents")]
        public Contents Contents { get; set; }

        [JsonProperty("ResultCount")]
        public long ResultCount { get; set; }

        [JsonProperty("ObjectUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ObjectUrl { get; set; }

        [JsonProperty("Id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }

        [JsonProperty("Name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("BackgroundUrl")]
        public object BackgroundUrl { get; set; }

        [JsonProperty("DefaultIndex")]
        public long DefaultIndex { get; set; }

        [JsonProperty("HighlightedContentNumber")]
        public long HighlightedContentNumber { get; set; }

        [JsonProperty("ImageIdentifier")]
        public object ImageIdentifier { get; set; }

        [JsonProperty("IsSelection")]
        public bool IsSelection { get; set; }

        [JsonProperty("SelectionIndex")]
        public long SelectionIndex { get; set; }

        [JsonProperty("ShowAll")]
        public bool ShowAll { get; set; }

        [JsonProperty("ShowInMenu")]
        public bool ShowInMenu { get; set; }

        [JsonProperty("ViewType")]
        public long ViewType { get; set; }
    }
}