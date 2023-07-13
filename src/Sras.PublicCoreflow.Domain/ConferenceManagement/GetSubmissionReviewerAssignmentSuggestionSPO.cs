using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class GetSubmissionReviewerAssignmentSuggestionSPO
    {
        public Guid? ReviewerId { get; set; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Organization { get; set; }
        public string? SelectedSubmissionConflicts { get; set; }
        public string? SelectedReviewerConflicts { get; set; }
        public string? SelectedReviewerSubjectAreas { get; set; }
        public int? Quota { get; set; }
        public bool? IsAssigned { get; set; }
        public int? NumberOfAssignments { get; set; }
    }
}
