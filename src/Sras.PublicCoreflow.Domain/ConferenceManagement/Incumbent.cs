using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Incumbent : FullAuditedAggregateRoot<Guid>
    {
        public Guid ConferenceAccountId { get; private set; }
        public ConferenceAccount ConferenceAccount { get; private set; }
        public Guid ConferenceRoleId { get; private set; }
        public ConferenceRole ConferenceRole { get; private set; }
        public Guid? TrackId { get; private set; }
        public Track? Track { get; private set; }
        public bool IsPrimaryContact { get; set; }

        public ICollection<Conflict> Conflicts { get; set; }
        public ICollection<Submission> CreationSubmissions { get; set; }
        public ICollection<Submission> ModificationSubmissions { get; set; }

        public Incumbent(
            Guid id,
            Guid conferenceAccountId,
            Guid conferenceRoleId,
            Guid? trackId,
            bool isPrimaryContact) : base(id)
        {
            ConferenceAccountId = conferenceAccountId;
            ConferenceRoleId = conferenceRoleId;
            TrackId = trackId;
            IsPrimaryContact = isPrimaryContact;

            Conflicts = new Collection<Conflict>();
            CreationSubmissions = new Collection<Submission>();
            ModificationSubmissions = new Collection<Submission>();
        }
    }
}
