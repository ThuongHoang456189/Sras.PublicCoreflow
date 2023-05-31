using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class InvitationClone : FullAuditedAggregateRoot<Guid>
    {
        public Guid EmailId { get; private set; }
        public Invitation Invitation { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public string InviteeUrl { get; private set; }
        public string InviteeAcceptUrl { get; private set; }
        public string InviteeDeclineUrl { get; private set; }

        public InvitationClone(Guid id, Guid emailId, DateTime expirationDate, string inviteeUrl, string inviteeAcceptUrl, string inviteeDeclineUrl) : base(id)
        {
            EmailId = emailId;
            ExpirationDate = expirationDate;
            InviteeUrl = inviteeUrl;
            InviteeAcceptUrl = inviteeAcceptUrl;
            InviteeDeclineUrl = inviteeDeclineUrl;
        }
    }
}
