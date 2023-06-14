using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionReviewerAssignmentSuggestion
    {
        public Guid TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public Guid SubmissionId { get; set; }
        public string SubmissionTitle { get; set; } = string.Empty;
        public List<SubmissionSubjectAreaBriefInfo> SubmissionSubjectAreas { get; set; } = new List<SubmissionSubjectAreaBriefInfo>();
        public List<ReviewerWithFacts> Reviewers { get; set; } = new List<ReviewerWithFacts>();
    }
}
