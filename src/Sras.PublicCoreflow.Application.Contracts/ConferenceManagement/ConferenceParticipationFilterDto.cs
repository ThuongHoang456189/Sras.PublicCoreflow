using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceParticipationFilterDto 
    {
        public Guid? TrackId { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
    }
}
