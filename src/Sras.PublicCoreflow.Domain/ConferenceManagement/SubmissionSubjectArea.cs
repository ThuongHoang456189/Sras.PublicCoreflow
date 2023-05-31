using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionSubjectArea : FullAuditedAggregateRoot<Guid>
    {
        public Guid SubmissionId { get; private set; }
        public Submission Submission { get; private set; }
        public Guid SubjectAreaId { get; private set; }
        public SubjectArea SubjectArea { get; private set; }
        public bool IsPrimary { get; private set; }

        public SubmissionSubjectArea(Guid id, Guid submissionId, Guid subjectAreaId, bool isPrimary) : base(id) 
        {
            SubmissionId = submissionId;
            SubjectAreaId = subjectAreaId;
            IsPrimary = isPrimary;
        }
    }
}
