using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AuthorSubmission
    {
        public Guid SubmissionId { get; set; }
        public string? SubmissionTitle { get; set; }
        public Guid TrackId { get; set; }
        public string? TrackName { get; set; }
        public Guid NotifiedStatusId { get; set; }
        public string? NotifiedStatusName { get; set; }
    }
}
