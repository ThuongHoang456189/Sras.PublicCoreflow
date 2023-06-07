using System;
using System.Collections.Generic;
namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceAccountDto
    {
        public Guid ConferenceId { get; set; }
        public Guid AccountId { get; set; }
        public bool HasDomainConflictConfirmed { get; set; }

        public List<IncumbentDto> Incumbents { get; set; }
    }
}
