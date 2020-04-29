using System;
using System.Collections.Generic;

namespace GO_Player.Models.API
{
    using J = Newtonsoft.Json.JsonPropertyAttribute;
    using R = Newtonsoft.Json.Required;
    using N = Newtonsoft.Json.NullValueHandling;

    public partial class Login
    {
        [J("Token", NullValueHandling = N.Ignore)] public string Token { get; set; }
        [J("SessionId", NullValueHandling = N.Ignore)] public Guid? SessionId { get; set; }
        [J("Status", NullValueHandling = N.Ignore)] public long? Status { get; set; }
        [J("Customer", NullValueHandling = N.Ignore)] public Customer Customer { get; set; }
        [J("Action", NullValueHandling = N.Ignore)] public string Action { get; set; }
        [J("AppLanguage")] public object AppLanguage { get; set; }
        [J("ActivationCode")] public object ActivationCode { get; set; }
        [J("AllowedContents")] public object AllowedContents { get; set; }
        [J("AudioLanguage")] public object AudioLanguage { get; set; }
        [J("AutoPlayNext", NullValueHandling = N.Ignore)] public bool? AutoPlayNext { get; set; }
        [J("BirthYear", NullValueHandling = N.Ignore)] public long? BirthYear { get; set; }
        [J("CurrentDevice", NullValueHandling = N.Ignore)] public CurrentDevice CurrentDevice { get; set; }
        [J("CustomerCode")] public object CustomerCode { get; set; }
        [J("DebugMode", NullValueHandling = N.Ignore)] public bool? DebugMode { get; set; }
        [J("DefaultSubtitleLanguage")] public object DefaultSubtitleLanguage { get; set; }
        [J("EmailAddress", NullValueHandling = N.Ignore)] public string EmailAddress { get; set; }
        [J("FirstName")] public object FirstName { get; set; }
        [J("Gender", NullValueHandling = N.Ignore)] public long? Gender { get; set; }
        [J("Id", NullValueHandling = N.Ignore)] public Guid? Id { get; set; }
        [J("IsAnonymus", NullValueHandling = N.Ignore)] public bool? IsAnonymus { get; set; }
        [J("IsPromo", NullValueHandling = N.Ignore)] public bool? IsPromo { get; set; }
        [J("Language", NullValueHandling = N.Ignore)] public string Language { get; set; }
        [J("LastName")] public object LastName { get; set; }
        [J("Nick")] public object Nick { get; set; }
        [J("NotificationChanges", NullValueHandling = N.Ignore)] public long? NotificationChanges { get; set; }
        [J("OperatorId", NullValueHandling = N.Ignore)] public Guid? OperatorId { get; set; }
        [J("OperatorName")] public object OperatorName { get; set; }
        [J("OperatorToken")] public object OperatorToken { get; set; }
        [J("ParentalControl")] public object ParentalControl { get; set; }
        [J("Password", NullValueHandling = N.Ignore)] public string Password { get; set; }
        [J("PromoCode")] public object PromoCode { get; set; }
        [J("ReferenceId", NullValueHandling = N.Ignore)] public Guid? ReferenceId { get; set; }
        [J("SecondaryEmailAddress")] public object SecondaryEmailAddress { get; set; }
        [J("SecondarySpecificData")] public object SecondarySpecificData { get; set; }
        [J("ServiceCode")] public object ServiceCode { get; set; }
        [J("SubscribeForNewsletter", NullValueHandling = N.Ignore)] public bool? SubscribeForNewsletter { get; set; }
        [J("SubscState")] public object SubscState { get; set; }
        [J("SubtitleSize")] public object SubtitleSize { get; set; }
        [J("TVPinCode")] public object TvPinCode { get; set; }
        [J("ZipCode")] public object ZipCode { get; set; }
    }

    public partial class CurrentDevice
    {
        [J("AppLanguage")] public object AppLanguage { get; set; }
        [J("AutoPlayNext")] public bool AutoPlayNext { get; set; }
        [J("Id")] public Guid Id { get; set; }
        [J("Individualization")] public Guid Individualization { get; set; }
        [J("Name")] public string Name { get; set; }
        [J("Platform")] public object Platform { get; set; }
        [J("CreatedDate")] public object CreatedDate { get; set; }
        [J("DeletedDate")] public object DeletedDate { get; set; }
        [J("IsDeleted")] public bool IsDeleted { get; set; }
        [J("OSName")] public object OsName { get; set; }
        [J("OSVersion")] public object OsVersion { get; set; }
        [J("Brand")] public object Brand { get; set; }
        [J("Modell")] public object Modell { get; set; }
        [J("SWVersion")] public object SwVersion { get; set; }
        [J("LastUsed")] public object LastUsed { get; set; }
        [J("SubtitleSize")] public object SubtitleSize { get; set; }
    }

    public partial class Customer
    {
        [J("Id")] public Guid Id { get; set; }
        [J("Action")] public string Action { get; set; }
        [J("OperatorId")] public Guid OperatorId { get; set; }
        [J("IsAnonymus")] public bool IsAnonymus { get; set; }
        [J("EmailAddress")] public string EmailAddress { get; set; }
        [J("Nick")] public string Nick { get; set; }
        [J("BirthYear")] public long BirthYear { get; set; }
        [J("SubscribeForNewsletter")] public bool SubscribeForNewsletter { get; set; }
        [J("IpAddress")] public string IpAddress { get; set; }
        [J("OperatorName")] public string OperatorName { get; set; }
        [J("Language")] public string Language { get; set; }
        [J("CustomerCode")] public string CustomerCode { get; set; }
        [J("ServiceCode")] public string ServiceCode { get; set; }
        [J("CountryName")] public string CountryName { get; set; }
        [J("ParentId")] public Guid ParentId { get; set; }
        [J("AppLanguage")] public string AppLanguage { get; set; }
        [J("AudioLanguage")] public string AudioLanguage { get; set; }
        [J("DefaultSubtitleLanguage")] public string DefaultSubtitleLanguage { get; set; }
        [J("AutoPlayNext")] public bool AutoPlayNext { get; set; }
        [J("ProfileName")] public string ProfileName { get; set; }
        [J("SubtitleSize")] public string SubtitleSize { get; set; }
        [J("IsDefaultProfile")] public bool IsDefaultProfile { get; set; }
        [J("Gender")] public long Gender { get; set; }
        [J("ZipCode")] public string ZipCode { get; set; }
        [J("FirstName")] public string FirstName { get; set; }
        [J("LastName")] public string LastName { get; set; }
        [J("PromoId")] public string PromoId { get; set; }
        [J("SubscState")] public string SubscState { get; set; }
        [J("CurrentDevice")] public CurrentDevice CurrentDevice { get; set; }
    }

    public partial class Operators
    {
        [J("Items")] public List<Item> Items { get; set; }
        [J("Host", NullValueHandling = N.Ignore)] public string Host { get; set; }
    }

    public partial class Item
    {
        [J("Id")] public Guid Id { get; set; }
        [J("Name")] public string Name { get; set; }
        [J("IsPromoted")] public bool IsPromoted { get; set; }
        [J("PopupHeight")] public long PopupHeight { get; set; }
        [J("LiveStreamSupport")] public bool LiveStreamSupport { get; set; }
        [J("DownloadSupport")] public bool DownloadSupport { get; set; }
        [J("DefaultCommercialStatus")] public long DefaultCommercialStatus { get; set; }
        [J("AuthenticationType")] public long AuthenticationType { get; set; }
        [J("IsOwn")] public long IsOwn { get; set; }
        [J("Description")] public string Description { get; set; }
        [J("RegistrationPageDisabled", NullValueHandling = N.Ignore)] public bool? RegistrationPageDisabled { get; set; }
        [J("WeightIndex")] public long WeightIndex { get; set; }
        [J("HideFromRegistration")] public bool HideFromRegistration { get; set; }
        [J("Parameters")] public List<Parameter> Parameters { get; set; }
        [J("Templates")] public List<object> Templates { get; set; }
        [J("Type", NullValueHandling = N.Ignore)] public string Type { get; set; }
        [J("HelpTextOperator", NullValueHandling = N.Ignore)] public string HelpTextOperator { get; set; }
        [J("LogoUrl", NullValueHandling = N.Ignore)] public string LogoUrl { get; set; }
        [J("WebsiteUrl", NullValueHandling = N.Ignore)] public string WebsiteUrl { get; set; }
        [J("CountryId", NullValueHandling = N.Ignore)] public Guid? CountryId { get; set; }
        [J("RedirectionUrl", NullValueHandling = N.Ignore)] public string RedirectionUrl { get; set; }
    }

    public partial class Parameter
    {
        [J("ParameterType")] public long ParameterType { get; set; }
        [J("VisibleRegistration")] public bool VisibleRegistration { get; set; }
        [J("VisibleParentalControl")] public bool VisibleParentalControl { get; set; }
        [J("VisibleProfile")] public bool VisibleProfile { get; set; }
        [J("VisiblePasswordChange")] public bool VisiblePasswordChange { get; set; }
        [J("VisibleLogin")] public bool VisibleLogin { get; set; }
        [J("IsRequired")] public bool IsRequired { get; set; }
        [J("MustBeEqualWithParameter")] public long MustBeEqualWithParameter { get; set; }
        [J("IsEditableOnProfile")] public bool IsEditableOnProfile { get; set; }
        [J("Name")] public string Name { get; set; }
        [J("ValidationExpression", NullValueHandling = N.Ignore)] public string ValidationExpression { get; set; }
        [J("Help")] public string Help { get; set; }
        [J("InputType")] public long InputType { get; set; }
        [J("ValidationErrorMessage")] public string ValidationErrorMessage { get; set; }
        [J("VisibleShortRegistration")] public bool VisibleShortRegistration { get; set; }
        [J("VisiblePasswordReset")] public bool VisiblePasswordReset { get; set; }
        [J("Hidden", NullValueHandling = N.Ignore)] public bool? Hidden { get; set; }
        [J("CompareErrorMessage")] public string CompareErrorMessage { get; set; }
        [J("DefaultValue", NullValueHandling = N.Ignore)] public string DefaultValue { get; set; }
        [J("RequiredErrorMessage")] public string RequiredErrorMessage { get; set; }
    }
}
