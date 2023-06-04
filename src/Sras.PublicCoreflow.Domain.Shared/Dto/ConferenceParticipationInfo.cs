using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceParticipationInfo : AccountWithBriefInfo
    {
        public List<RoleWithEngagedTrackInfo>? Roles { get; set; }

        public ConferenceParticipationInfo() : base() { }
    }
}
