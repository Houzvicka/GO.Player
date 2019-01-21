using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Customer
    {
        [JsonProperty("Action")]
        public string Action { get; set; }

        [JsonProperty("AllowedContents")]
        public List<object> AllowedContents { get; set; }

        [JsonProperty("BirthYear")]
        public long BirthYear { get; set; }

        [JsonProperty("IpAddress")]
        public object IpAddress { get; set; }

        [JsonProperty("CurrentDevice")]
        public CurrentDevice CurrentDevice { get; set; }

        [JsonProperty("CustomerCode")]
        public object CustomerCode { get; set; }

        [JsonProperty("DebugMode")]
        public bool DebugMode { get; set; }

        [JsonProperty("EmailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("Gender")]
        public long Gender { get; set; }

        [JsonProperty("GroupIndexes")]
        public object GroupIndexes { get; set; }

        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("IsAnonymus")]
        public bool IsAnonymus { get; set; }

        [JsonProperty("Nick")]
        public string Nick { get; set; }

        [JsonProperty("OperatorId")]
        public Guid OperatorId { get; set; }

        [JsonProperty("OperatorName")]
        public string OperatorName { get; set; }

        [JsonProperty("OperatorToken")]
        public object OperatorToken { get; set; }

        [JsonProperty("ParentId")]
        public Guid ParentId { get; set; }

        [JsonProperty("ParentalControl")]
        public object ParentalControl { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("SecondarySpecificData")]
        public object SecondarySpecificData { get; set; }

        [JsonProperty("SpecificData")]
        public object SpecificData { get; set; }

        [JsonProperty("SubscribeForNewsletter")]
        public bool SubscribeForNewsletter { get; set; }

        [JsonProperty("TVPinCode")]
        public object TvPinCode { get; set; }

        [JsonProperty("ZipCode")]
        public string ZipCode { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("CountryName")]
        public string CountryName { get; set; }

        [JsonProperty("ServiceCode")]
        public string ServiceCode { get; set; }

        [JsonProperty("AppLanguage")]
        public string AppLanguage { get; set; }

        [JsonProperty("AudioLanguage")]
        public string AudioLanguage { get; set; }

        [JsonProperty("DefaultSubtitleLanguage")]
        public string DefaultSubtitleLanguage { get; set; }

        [JsonProperty("AutoPlayNext")]
        public bool AutoPlayNext { get; set; }

        [JsonProperty("ProfileName")]
        public string ProfileName { get; set; }

        [JsonProperty("SubtitleSize")]
        public string SubtitleSize { get; set; }

        [JsonProperty("IsDefaultProfile")]
        public bool IsDefaultProfile { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("PromoId")]
        public string PromoId { get; set; }

        [JsonProperty("SubscState")]
        public string SubscState { get; set; }
    }
}