using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Registration : FullAuditedAggregateRoot<Guid>
    {
        [ForeignKey(nameof(Creator))]
        public Guid CreatedId { get; private set; }
        public ConferenceAccount Creator { get; private set; }

        public ICollection<RegistrationPaper> RegistrationPapers { get; set; }

        public Registration(Guid id, Guid createdId) : base(id)
        {
            CreatedId = createdId;

            RegistrationPapers = new Collection<RegistrationPaper>();
        }
    }
}
