using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionBriefInfo
    {
        public Guid SubmissionId { get; set; }
        public string? SubmissionTitle { get; set; }
        public Guid TrackId { get; set; }
        public string? TrackName { get; set; }
    }
}
