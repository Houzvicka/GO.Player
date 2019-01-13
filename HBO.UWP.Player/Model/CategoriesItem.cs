using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
{
    public partial class CategoriesItem
    {
        [JsonProperty("ObjectType")]
        public long ObjectType { get; set; }

        [JsonProperty("ObjectUrl")]
        public string ObjectUrl { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Color")]
        public string Color { get; set; }

        [JsonProperty("Container")]
        public List<Container> Container { get; set; }

        [JsonProperty("ExpiryMin")]
        public long ExpiryMin { get; set; }

        [JsonProperty("Filters")]
        public List<Filter> Filters { get; set; }

        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("IsSelection")]
        public bool IsSelection { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("SelectionIndex")]
        public long SelectionIndex { get; set; }

        [JsonProperty("ShowInMenu")]
        public bool ShowInMenu { get; set; }

        [JsonProperty("SortOptions")]
        public List<SortOption> SortOptions { get; set; }

        [JsonProperty("Tracking")]
        public FluffyTracking Tracking { get; set; }

        [JsonProperty("ViewType")]
        public long ViewType { get; set; }

        [JsonProperty("Locked", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Locked { get; set; }
    }
}