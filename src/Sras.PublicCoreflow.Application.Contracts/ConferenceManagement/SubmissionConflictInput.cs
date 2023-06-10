using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionConflictInput
    {
        public Guid SubmissionId { get; set; }
        public List<ConflictInput> Conflicts { get; set; } = new List<ConflictInput>();
    }
}
