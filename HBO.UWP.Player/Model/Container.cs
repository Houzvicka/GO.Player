using System;
using Newtonsoft.Json;

namespace HBO.UWP.Player.Model
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
    }
}