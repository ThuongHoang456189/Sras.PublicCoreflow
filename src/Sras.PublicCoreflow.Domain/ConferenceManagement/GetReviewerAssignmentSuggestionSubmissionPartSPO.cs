using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class GetReviewerAssignmentSuggestionSubmissionPartSPO
    {
        public Guid? PaperId { get; set; }
        public string? Title { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public string? SubjectAreaRelevanceCoefficients { get; set; }
        public string? SelectedSubmissionSubjectAreas { get; set; }
    }
}
