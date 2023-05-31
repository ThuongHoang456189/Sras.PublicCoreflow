using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class PlaceholderGroup : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }

        public ICollection<SupportedPlaceholder> SupportedPlaceholders { get; private set; }

        public PlaceholderGroup(Guid id, string name) : base(id)
        {
            Name = name;

            SupportedPlaceholders = new Collection<SupportedPlaceholder>();
        }
    }
}
