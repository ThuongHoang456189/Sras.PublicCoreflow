using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class EmailTemplate : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public Guid? ConferenceId { get; set; }
        public Conference? Conference { get; set; }
        public Guid? TrackId { get; set; }
        public Track? Track { get; set; }

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