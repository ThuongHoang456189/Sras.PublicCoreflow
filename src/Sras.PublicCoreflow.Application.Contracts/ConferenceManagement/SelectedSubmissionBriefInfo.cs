using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SelectedSubmissionBriefInfo
    {
        public Guid SubmissionId { get; set; }
        public string Title { get; set; }
        public Guid TrackId { get; set; }
        public string TrackName { get; set; }
    }
}
