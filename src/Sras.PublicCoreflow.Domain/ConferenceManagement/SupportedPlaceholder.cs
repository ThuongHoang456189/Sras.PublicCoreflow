using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SupportedPlaceholder : FullAuditedAggregateRoot<Guid>
    {
        public string Encode { get; private set; }
        public string Description { get; private set; }
        public Guid PlaceholderGroupId { get; private set; }
        public PlaceholderGroup PlaceholderGroup { get; private set; }
        public Guid EmailTemplateId { get; private set; }
        public EmailTemplate EmailTemplate { get; private set; }

        public SupportedPlaceholder(Guid id, string encode, string description, Guid placeholderGroupId, Guid emailTemplateId) : base(id)
        {
            Encode = encode;
            Description = description;
            PlaceholderGroupId = placeholderGroupId;
            EmailTemplateId = emailTemplateId;
        }
    }
}