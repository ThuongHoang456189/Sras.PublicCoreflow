using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Participant : FullAuditedAggregateRoot<Guid>
    {
        public ICollection<IdentityUser>? Accounts { get; private set; }
        public ICollection<Outsider>? OutsiderInformations { get; private set; }

        public Participant()
        {

        }
    }
}
