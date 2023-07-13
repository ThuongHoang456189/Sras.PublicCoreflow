using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionReviewerAssignmentSuggestionInput
    {
        public string? InclusionText { get; set; }
        public Guid SubmissionId { get; set; }
        public bool? IsAssigned { get; set; }
        public int? SkipCount { get; set; }
        public int? MaxResultCount { get; set; }
    }
}
