using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConflictCase : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public bool IsIndividual { get; private set; }
        public bool IsDefault { get; private set; }
        public Guid TrackId { get; private set; }
        public Track Track { get; private set; }

        public ICollection<Conflict> Conflicts { get; private set; }

        public ConflictCase (Guid id, string name, bool isIndividual, bool isDefault, Guid trackId) : base (id)
        {
            Name = name;
            IsIndividual = isIndividual;
            IsDefault = isDefault;
            TrackId = trackId;

            Conflicts = new Collection<Conflict> ();
        }
    }
}
