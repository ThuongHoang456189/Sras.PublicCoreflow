using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{

    public class Invitation : FullAuditedAggregateRoot<Guid>
    {
        [ForeignKey(nameof(Email))]
        public Guid Id { get; private set; }
        public virtual Email Email { get; private set; }
        public byte Status { get; private set; }
        public string? Comment { get; private set; }
        public bool IsReviewer { get; private set; }

        public ICollection<InvitationClone> Clones { get; private set; }

        public Invitation(Guid id, byte status, string? comment, bool isReviewer) : base(id)
        {
            Status = status;
            Comment = comment;
            IsReviewer = isReviewer;

            Clones = new Collection<InvitationClone>();
        }
    }
}
