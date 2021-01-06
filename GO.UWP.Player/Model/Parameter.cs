using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class Parameter
    {
        [JsonProperty("ParameterType")]
        public long ParameterType { get; set; }

        [JsonProperty("VisibleRegistration")]
        public bool VisibleRegistration { get; set; }

        [JsonProperty("VisibleParentalControl")]
        public bool VisibleParentalControl { get; set; }

        [JsonProperty("VisibleProfile")]
        public bool VisibleProfile { get; set; }

        [JsonProperty("VisiblePasswordChange")]
        public bool VisiblePasswordChange { get; set; }

        [JsonProperty("VisibleLogin")]
        public bool VisibleLogin { get; set; }

        [JsonProperty("IsRequired")]
        public bool IsRequired { get; set; }

        [JsonProperty("MustBeEqualWithParameter")]
        public long MustBeEqualWithParameter { get; set; }

        [JsonProperty("IsEditableOnProfile")]
        public bool IsEditableOnProfile { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ValidationExpression", NullValueHandling = NullValueHandling.Ignore)]
        public string ValidationExpression { get; set; }

        [JsonProperty("Help", NullValueHandling = NullValueHandling.Ignore)]
        public string Help { get; set; }

        [JsonProperty("InputType")]
        public long InputType { get; set; }

        [JsonProperty("ValidationErrorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ValidationErrorMessage { get; set; }

        [JsonProperty("VisibleShortRegistration")]
        public bool VisibleShortRegistration { get; set; }

        [JsonProperty("VisiblePasswordReset")]
        public bool VisiblePasswordReset { get; set; }

        [JsonProperty("Hidden")]
        public bool Hidden { get; set; }
    }
}