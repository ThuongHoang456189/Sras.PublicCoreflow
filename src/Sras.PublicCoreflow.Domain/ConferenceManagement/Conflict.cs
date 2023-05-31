using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Conflict : FullAuditedAggregateRoot
    {
        [ForeignKey(nameof(Submission))]
        public Guid SubmissionId { get; private set; }  
        public Submission Submission { get; private set; }
        [ForeignKey(nameof(Incumbent))]
        public Guid IncumbentId { get; private set; }
        public Incumbent Incumbent { get; private set; }
        [ForeignKey(nameof(ConflictCase))]
        public Guid ConflictCaseId { get; private set; }
        public ConflictCase ConflictCase { get; private set; }
        public bool IsDefinedByReviewer { get; private set; }

        public Conflict(Guid submissionId, Guid incumbentId, Guid conflictCaseId, bool isDefinedByReviewer) 
        {
            SubmissionId = submissionId;
            IncumbentId = incumbentId;
            ConflictCaseId = conflictCaseId;
            IsDefinedByReviewer = isDefinedByReviewer;
        }

        public override object[] GetKeys()
        {
            return new object[] { SubmissionId, IncumbentId, ConflictCaseId };
        }
    }
}
