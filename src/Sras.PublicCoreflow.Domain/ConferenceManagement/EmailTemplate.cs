using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class EmailTemplate : FullAuditedAggregateRoot<Guid>
    {
        public string Subject { get; private set; }
        public string Body { get; private set; }

        public ICollection<Email> Emails { get; private set; }
        public ICollection<SupportedPlaceholder> SupportedPlaceholders { get; private set; }

        public EmailTemplate(Guid id, string subject, string body) : base(id)
        {
            Subject = subject;
            Body = body;

            Emails = new Collection<Email>();
            SupportedPlaceholders = new Collection<SupportedPlaceholder>();
        }
    }
}