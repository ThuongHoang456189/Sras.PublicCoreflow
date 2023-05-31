using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceReviewerSubjectArea : FullAuditedAggregateRoot<Guid>
    {
        public Guid ConferenceReviewerId { get; private set; }
        public ConferenceReviewer ConferenceReviewer { get; private set; }
        public Guid SubjectAreaId { get; private set; }
        public SubjectArea SubjectArea { get; private set; }
        public bool IsPrimary { private set; get; }

        public ConferenceReviewerSubjectArea(Guid id, Guid conferenceReviewerId, Guid subjectAreaId, bool isPrimary) : base(id) 
        {
            ConferenceReviewerId = conferenceReviewerId;
            SubjectAreaId = subjectAreaId;
            IsPrimary = isPrimary;
        }
    }
}
