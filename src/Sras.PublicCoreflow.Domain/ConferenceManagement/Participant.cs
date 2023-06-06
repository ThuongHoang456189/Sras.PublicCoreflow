using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Participant : FullAuditedAggregateRoot<Guid>
    {
        public Guid? AccountId { get; private set; }
        public IdentityUser? Account { get; private set; }
        public Guid? OutsiderId { get; private set; }
        public Outsider? Outsider { get; private set; }
        public ICollection<Author> Authors { get; private set; }

        public Participant(Guid id, Guid? accountId, Guid? outsiderId) : base(id)
        {
            AccountId = accountId;
            OutsiderId = outsiderId;
            Authors = new Collection<Author>();
        }
    }
}
