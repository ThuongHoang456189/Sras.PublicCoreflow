using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerSubjectArea : FullAuditedAggregateRoot
    {
        [ForeignKey(nameof(Reviewer))]
        public Guid ReviewerId { get; private set; }
        public virtual Reviewer Reviewer { get; private set; }
        [ForeignKey(nameof(SubjectArea))]
        public Guid SubjectAreaId { get; private set; }
        public virtual SubjectArea SubjectArea { get; private set; }
        public bool IsPrimary { set; get; }

        public ReviewerSubjectArea(Guid reviewerId, Guid subjectAreaId, bool isPrimary)
            : base()
        {
            ReviewerId = reviewerId;
            SubjectAreaId = subjectAreaId;
            IsPrimary = isPrimary;
        }

        public override object[] GetKeys()
        {
            return new object[] { ReviewerId, SubjectAreaId };
        }
    }
}
