using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Participant : FullAuditedAggregateRoot<Guid>
    {
        public ICollection<IdentityUser> Accounts { get; private set; }
        public ICollection<Outsider> Outsiders { get; private set; }
        public ICollection<Author> Authors { get; private set; }

        public Participant(Guid id) : base(id)
        {
            Accounts = new Collection<IdentityUser>();
            Outsiders = new Collection<Outsider>();
            Authors = new Collection<Author>();
        }
    }
}
