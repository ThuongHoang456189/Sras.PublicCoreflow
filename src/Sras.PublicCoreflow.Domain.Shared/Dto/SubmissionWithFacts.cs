using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionWithFacts
    {
        public Guid ReviewAssignmentId { get; set; }
        public Guid SubmissionId { get; set; }
        public string? SubmissionTitle { get; set; }
        public Guid TrackId { get; set; }
        public string? TrackName { get; set; }
        public Guid ReviewerId { get; set; }
        public List<SelectedSubjectAreaBriefInfo>? SubmissionSubjectAreas { get; set; }
        public double Relevance { get; set; }
    }
}
