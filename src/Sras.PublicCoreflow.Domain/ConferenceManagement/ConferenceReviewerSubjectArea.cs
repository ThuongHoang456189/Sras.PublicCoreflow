using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceReviewerSubjectArea : FullAuditedAggregateRoot
    {
        [ForeignKey(nameof(ConferenceReviewer))]
        public Guid ConferenceReviewerId { get; private set; }
        public virtual ConferenceReviewer ConferenceReviewer { get; private set; }
        [ForeignKey(nameof(SubjectArea))]
        public Guid SubjectAreaId { get; private set; }
        public virtual SubjectArea SubjectArea { get; private set; }
        public bool IsPrimary { private set; get; }

        public ConferenceReviewerSubjectArea(Guid conferenceReviewerId, Guid subjectAreaId, bool isPrimary)
        {
            ConferenceReviewerId = conferenceReviewerId;
            SubjectAreaId = subjectAreaId;
            IsPrimary = isPrimary;
        }

        public override object[] GetKeys()
        {
            return new object[] { ConferenceReviewerId, SubjectAreaId };
        }
    }
}
