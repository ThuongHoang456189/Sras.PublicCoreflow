using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerBriefInformationDto
    {
        public Guid? AccountId { get; set; }
        public string? NamePrefix { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Organization { get; set; }
        public string? Country { get; set; }
        public string? DomainConflicts { get; set; }
    }
}
