using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerConflictInput
    {
        public Guid AccountId { get; set; }
        public Guid ConferenceId { get; set; }
        public Guid SubmissionId { get; set; }
        public List<Guid> ConflictCases { get; set; } = new List<Guid>();
    }
}
