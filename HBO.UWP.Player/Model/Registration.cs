namespace HBO.UWP.Player.Model
{
    using Newtonsoft.Json;

    public partial class Registration
    {
        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Data")]
        public Data Data { get; set; }
    }
}