using System;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class ItemTracking
    {
        [JsonProperty("AssetName")]
        public string AssetName { get; set; }

        [JsonProperty("EpisodeName")]
        public string EpisodeName { get; set; }

        [JsonProperty("EpisodeNumber")]
        public long EpisodeNumber { get; set; }

        [JsonProperty("ExternalId")]
        public string ExternalId { get; set; }

        [JsonProperty("Genre")]
        public string Genre { get; set; }

        [JsonProperty("IsLive")]
        [JsonConverter(typeof(ParseStringConverter))]
        public bool IsLive { get; set; }

        [JsonProperty("IsTrailer")]
        [JsonConverter(typeof(ParseStringConverter))]
        public bool IsTrailer { get; set; }

        [JsonProperty("SeasonNumber")]
        public long SeasonNumber { get; set; }

        [JsonProperty("SecondaryGenre")]
        public string SecondaryGenre { get; set; }

        [JsonProperty("ShowName")]
        public string ShowName { get; set; }
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            bool b;
            if (Boolean.TryParse(value, out b))
            {
                return b;
            }
            throw new Exception("Cannot unmarshal type bool");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (bool)untypedValue;
            var boolString = value ? "true" : "false";
            serializer.Serialize(writer, boolString);
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}