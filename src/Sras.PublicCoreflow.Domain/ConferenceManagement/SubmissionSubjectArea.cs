using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionSubjectArea : FullAuditedAggregateRoot
    {
        [ForeignKey(nameof(Submission))]
        public Guid SubmissionId { get; private set; }
        public Submission Submission { get; private set; }
        [ForeignKey(nameof(SubjectArea))]
        public Guid SubjectAreaId { get; private set; }
        public SubjectArea SubjectArea { get; private set; }
        public bool IsPrimary { get; private set; }

        public SubmissionSubjectArea(Guid submissionId, Guid subjectAreaId, bool isPrimary)
        {
            SubmissionId = submissionId;
            SubjectAreaId = subjectAreaId;
            IsPrimary = isPrimary;
        }

        public override object[] GetKeys()
        {
            return new object[] { SubmissionId, SubjectAreaId };
        }
    }
}
