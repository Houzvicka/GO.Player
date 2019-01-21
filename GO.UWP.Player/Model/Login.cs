using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Login
    {
        [JsonProperty("Action")]
        public string Action { get; set; }

        [JsonProperty("AppLanguage")]
        public object AppLanguage { get; set; }

        [JsonProperty("ActivationCode")]
        public object ActivationCode { get; set; }

        [JsonProperty("AllowedContents")]
        public List<object> AllowedContents { get; set; }

        [JsonProperty("AudioLanguage")]
        public object AudioLanguage { get; set; }

        [JsonProperty("AutoPlayNext")]
        public bool AutoPlayNext { get; set; }

        [JsonProperty("BirthYear")]
        public long BirthYear { get; set; }

        [JsonProperty("CurrentDevice")]
        public CurrentDevice CurrentDevice { get; set; }

        [JsonProperty("CustomerCode")]
        public string CustomerCode { get; set; }

        [JsonProperty("DebugMode")]
        public bool DebugMode { get; set; }

        [JsonProperty("DefaultSubtitleLanguage")]
        public object DefaultSubtitleLanguage { get; set; }

        [JsonProperty("EmailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("Gender")]
        public long Gender { get; set; }

        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("IsAnonymus")]
        public bool IsAnonymus { get; set; }

        [JsonProperty("IsPromo")]
        public bool IsPromo { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Nick")]
        public string Nick { get; set; }

        [JsonProperty("NotificationChanges")]
        public long NotificationChanges { get; set; }

        [JsonProperty("OperatorId")]
        public Guid OperatorId { get; set; }

        [JsonProperty("OperatorName")]
        public string OperatorName { get; set; }

        [JsonProperty("OperatorToken")]
        public string OperatorToken { get; set; }

        [JsonProperty("ParentalControl")]
        public ParentalControl ParentalControl { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("PromoCode")]
        public string PromoCode { get; set; }

        [JsonProperty("ReferenceId")]
        public Guid ReferenceId { get; set; }

        [JsonProperty("SecondaryEmailAddress")]
        public string SecondaryEmailAddress { get; set; }

        [JsonProperty("SecondarySpecificData")]
        public object SecondarySpecificData { get; set; }

        [JsonProperty("ServiceCode")]
        public string ServiceCode { get; set; }

        [JsonProperty("SubscribeForNewsletter")]
        public bool SubscribeForNewsletter { get; set; }

        [JsonProperty("SubscState")]
        public object SubscState { get; set; }

        [JsonProperty("SubtitleSize")]
        public string SubtitleSize { get; set; }

        [JsonProperty("TVPinCode")]
        public string TvPinCode { get; set; }

        [JsonProperty("ZipCode")]
        public string ZipCode { get; set; }
    }
}
