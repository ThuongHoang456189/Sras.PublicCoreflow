using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceParticipationInput
    {
        public Guid AccountId { get; set; }
        public Guid ConferenceId { get; set; }
        public Guid? TrackId { get; set; }
    }
}
