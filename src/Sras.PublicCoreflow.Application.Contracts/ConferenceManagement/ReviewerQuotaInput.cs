using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerQuotaInput
    {
        public Guid AccountId { get; set; }
        public Guid ConferenceId { get; set; }
        public Guid TrackId { get; set; }
        public int? Quota { get; set; }
    }
}
