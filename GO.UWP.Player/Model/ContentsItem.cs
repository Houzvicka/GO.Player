using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GO.UWP.Player.Model
{
    public partial class ContentsItem
    {
        [JsonProperty("ObjectType")]
        public long ObjectType { get; set; }

        [JsonProperty("ObjectUrl")]
        public Uri ObjectUrl { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Abstract")]
        public string Abstract { get; set; }

        [JsonProperty("AdditionalName")]
        public string AdditionalName { get; set; }

        [JsonProperty("AgeRating")]
        public long AgeRating { get; set; }

        [JsonProperty("AgeRatingName")]
        public string AgeRatingName { get; set; }

        [JsonProperty("AllowFreePreview")]
        public bool AllowFreePreview { get; set; }

        [JsonProperty("AllowPlay")]
        public bool AllowPlay { get; set; }

        [JsonProperty("AvailabilityFrom")]
        public string AvailabilityFrom { get; set; }

        [JsonProperty("AvailabilityFromUtc")]
        public double AvailabilityFromUtc { get; set; }

        [JsonProperty("AvailabilityFromUtcIso")]
        public DateTimeOffset? AvailabilityFromUtcIso { get; set; }

        [JsonProperty("AvailabilityTo")]
        public string AvailabilityTo { get; set; }

        [JsonProperty("AvailabilityToUtc")]
        public double AvailabilityToUtc { get; set; }

        [JsonProperty("AvailabilityToUtcIso")]
        public DateTimeOffset? AvailabilityToUtcIso { get; set; }

        [JsonProperty("BackgroundUrl")]
        public Uri BackgroundUrl { get; set; }

        [JsonProperty("Cast")]
        public string Cast { get; set; }

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("ContentType")]
        public long ContentType { get; set; }

        [JsonProperty("CreditRollStart")]
        public long CreditRollStart { get; set; }

        [JsonProperty("Director")]
        public string Director { get; set; }

        [JsonProperty("Duration")]
        public long Duration { get; set; }

        [JsonProperty("DurationText")]
        public string DurationText { get; set; }

        [JsonProperty("EditedAbstract")]
        public string EditedAbstract { get; set; }

        [JsonProperty("EditedName")]
        public string EditedName { get; set; }

        [JsonProperty("ElapsedPercentage")]
        public long ElapsedPercentage { get; set; }

        [JsonProperty("EpisodeInSeries")]
        public long EpisodeInSeries { get; set; }

        [JsonProperty("ExternalId")]
        public string ExternalId { get; set; }

        [JsonProperty("FirstBurst")]
        public bool FirstBurst { get; set; }

        [JsonProperty("Genre")]
        public string Genre { get; set; }

        [JsonProperty("HasTrailer")]
        public bool HasTrailer { get; set; }

        [JsonProperty("HighlightInfo", NullValueHandling = NullValueHandling.Ignore)]
        public string HighlightInfo { get; set; }

        [JsonProperty("Highlighted")]
        public bool Highlighted { get; set; }

        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("ImageProperties")]
        public List<object> ImageProperties { get; set; }

        [JsonProperty("ImdbRate")]
        public double ImdbRate { get; set; }

        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("IsCollection")]
        public bool IsCollection { get; set; }

        [JsonProperty("IsDownloadable")]
        public bool IsDownloadable { get; set; }

        [JsonProperty("IsUpcoming")]
        public bool IsUpcoming { get; set; }

        [JsonProperty("LastBurst")]
        public bool LastBurst { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("OriginalName")]
        public string OriginalName { get; set; }

        [JsonProperty("ProductionYear")]
        public long ProductionYear { get; set; }

        [JsonProperty("SEOUrl")]
        public string SeoUrl { get; set; }

        [JsonProperty("SeasonIndex")]
        public long SeasonIndex { get; set; }

        [JsonProperty("SecondaryGenre")]
        public string SecondaryGenre { get; set; }

        [JsonProperty("SeriesName")]
        public string SeriesName { get; set; }

        [JsonProperty("Tracking")]
        public PurpleTracking Tracking { get; set; }

        [JsonProperty("UseDisclaimer")]
        public bool UseDisclaimer { get; set; }

        [JsonProperty("Writer")]
        public string Writer { get; set; }

        [JsonProperty("Availability", NullValueHandling = NullValueHandling.Ignore)]
        public string Availability { get; set; }

        [JsonProperty("OriginCountry", NullValueHandling = NullValueHandling.Ignore)]
        public string OriginCountry { get; set; }

        [JsonProperty("SeasonName", NullValueHandling = NullValueHandling.Ignore)]
        public string SeasonName { get; set; }

        [JsonProperty("AudioTracks", NullValueHandling = NullValueHandling.Ignore)]
        public List<AudioTrack> AudioTracks { get; set; }
        
        [JsonProperty("ChildContents", NullValueHandling = NullValueHandling.Ignore)]
        public ChildContents ChildContents { get; set; }
        
        [JsonProperty("Host", NullValueHandling = NullValueHandling.Ignore)]
        public string Host { get; set; }
        
        [JsonProperty("Subtitles", NullValueHandling = NullValueHandling.Ignore)]
        public List<Subtitle> Subtitles { get; set; }

        [JsonProperty("Parent", NullValueHandling = NullValueHandling.Ignore)]
        public ContentsItem Parent { get; set; }
    }
}