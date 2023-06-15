using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class RegistrablePaperTable
    {
        public Guid AccountId { get; set; }
        public string? Email { get; set; }
        public List<SubmissionBriefInfo>? RegistrablePapers { get; set; }
    }
}
