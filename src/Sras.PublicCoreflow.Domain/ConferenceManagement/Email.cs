using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Email : FullAuditedAggregateRoot<Guid>
    {
        public Guid SenderId { get; private set; }
        public Incumbent Sender { get; private set; }
        public Guid RecipientId { get; private set; }
        public Participant Recipient { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public Guid EmailTemplateId { get; private set; }
        public EmailTemplate EmailTemplate { get; private set; }

        public Email(Guid id, Guid senderId, Guid recipientId, string subject, string body, Guid emailTemplateId) : base(id)
        {
            SenderId = senderId;
            RecipientId = recipientId;
            Subject = subject;
            Body = body;
            EmailTemplateId = emailTemplateId;
        }
    }
}
