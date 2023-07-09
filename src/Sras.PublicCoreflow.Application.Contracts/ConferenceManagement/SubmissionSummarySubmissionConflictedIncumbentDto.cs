using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionSummarySubmissionConflictedIncumbentDto
    {
        public string? IncumbentNamePrefix { get; set; }
        public string? IncumbentFullName { get; set; }
        public string? IncumbentOrganization { get; set; }
        public string? IncumbentEmail { get; set; }
        public List<string>? Conflicts { get; set; }
    }
}
