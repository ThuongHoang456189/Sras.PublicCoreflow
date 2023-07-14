using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class GetReviewerReviewingInformationAggregationSPO
    {
        public int? TotalCount { get; set; }
        public Guid? AccountId { get; set; }
        public string? NamePrefix { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set;}
        public string? Email { get; set; }
        public string? Organization { get;set; }
        public string? Country { get; set; }
        public string? DomainConflicts { get; set; }
        public Guid? ConferenceId { get; set; }
        public string? ConferenceFullName { get; set; }
        public string? ConferenceShortName { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public Guid? ReviewerId { get; set; }
        public int? Quota { get; set; }
        public string? SelectedReviewerSubjectAreas { get; set; }
    }
}
