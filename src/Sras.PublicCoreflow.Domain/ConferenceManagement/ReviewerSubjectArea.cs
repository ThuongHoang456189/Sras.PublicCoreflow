using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerSubjectArea : FullAuditedAggregateRoot<Guid>
    {
        public Guid ReviewerId { get; private set; }
        public virtual Reviewer Reviewer { get; private set; }
        public Guid SubjectAreaId { get; private set; }
        public virtual SubjectArea SubjectArea { get; private set; }
        public bool IsPrimary { set; get; }

        public ReviewerSubjectArea(Guid id, Guid reviewerId, Guid subjectAreaId, bool isPrimary)
            : base(id)
        {
            ReviewerId = reviewerId;
            SubjectAreaId = subjectAreaId;
            IsPrimary = isPrimary;
        }
    }
}
