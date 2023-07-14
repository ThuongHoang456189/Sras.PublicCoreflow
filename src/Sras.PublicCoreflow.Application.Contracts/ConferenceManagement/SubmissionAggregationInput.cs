using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionAggregationInput
    {
        public string? InclusionText {  get; set; }
        public Guid ConferenceId { get; set; }
        public Guid? TrackId { get; set; }
        public Guid? StatusId { get; set; }
        public int? SkipCount { get; set; }
        public int? MaxResultCount { get; set; }
    }
}
