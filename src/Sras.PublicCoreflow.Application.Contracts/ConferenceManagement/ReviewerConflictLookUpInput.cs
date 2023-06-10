using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerConflictLookUpInput
    {
        public Guid AccountId { get; set; }
        public Guid ConferenceId { get; set; }
        public Guid TrackId { get; set; }
        public Guid SubmissionId { get; set; }
    }
}
