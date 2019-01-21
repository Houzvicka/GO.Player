using System;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class CurrentDevice
    {
        [JsonProperty("AppLanguage")]
        public string AppLanguage { get; set; }

        [JsonProperty("AutoPlayNext")]
        public bool AutoPlayNext { get; set; }

        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("Individualization")]
        public string Individualization { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Platform")]
        public string Platform { get; set; }

        [JsonProperty("CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("DeletedDate")]
        public string DeletedDate { get; set; }

        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("OSName")]
        public string OsName { get; set; }

        [JsonProperty("OSVersion")]
        public object OsVersion { get; set; }

        [JsonProperty("Brand")]
        public string Brand { get; set; }

        [JsonProperty("Modell")]
        public string Modell { get; set; }

        [JsonProperty("SWVersion")]
        public string SwVersion { get; set; }

        [JsonProperty("LastUsed")]
        public string LastUsed { get; set; }

        [JsonProperty("SubtitleSize")]
        public string SubtitleSize { get; set; }
    }
}