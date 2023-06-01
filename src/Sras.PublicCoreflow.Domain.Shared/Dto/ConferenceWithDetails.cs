using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceWithDetails : ConferenceWithBriefInfo
    {
        public List<ConferenceAccountDto> ConferenceAccounts { get; set; }
    }
}
