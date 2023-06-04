using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceParticipationBriefInfo : AccountWithBriefInfo
    {
        public List<string> Roles { get; set; } = new List<string>();

        public ConferenceParticipationBriefInfo() : base() { }
    }
}
