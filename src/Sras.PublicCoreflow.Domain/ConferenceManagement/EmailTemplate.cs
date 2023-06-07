using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class EmailTemplate : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }

        public Guid? ConferenceId { get; private set; }
        public Conference? Conference { get; private set; }
        public Guid? TrackId { get; private set; }
        public Track? Track { get; private set; }

        public ICollection<Email> Emails { get; private set; }

        public EmailTemplate(Guid id, string name, string subject, string body, Guid? conferenceId, Guid? trackId) : base(id)
        {
            Name = name;
            Subject = subject;
            Body = body;
            ConferenceId = conferenceId;
            TrackId = trackId;

            Emails = new Collection<Email>();
        }
    }
}