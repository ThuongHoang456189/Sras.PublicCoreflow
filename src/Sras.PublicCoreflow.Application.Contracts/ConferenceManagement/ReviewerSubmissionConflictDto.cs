using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerSubmissionConflictDto
    {
        public Guid SubmissionId { get; set; }
        public string? SubmissionTitle { get; set; }
        public Guid TrackId { get; set; }
        public string? TrackName { get; set; }
        public List<ConflictWithDetails>? Conflicts { get; set; }
    }
}
