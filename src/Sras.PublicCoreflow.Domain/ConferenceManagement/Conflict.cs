using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Conflict : FullAuditedAggregateRoot<Guid>
    {
        public Guid SubmissionId { get; private set; }  
        public Submission Submission { get; private set; }
        public Guid IncumbentId { get; private set; }
        public Incumbent Incumbent { get; private set; }
        public Guid ConflictCaseId { get; private set; }
        public ConflictCase ConflictCase { get; private set; }
        public bool IsDefinedByReviewer { get; private set; }

        public Conflict(Guid id, Guid submissionId, Guid incumbentId, Guid conflictCaseId, bool isDefinedByReviewer) : base(id) 
        {
            SubmissionId = submissionId;
            IncumbentId = incumbentId;
            ConflictCaseId = conflictCaseId;
            IsDefinedByReviewer = isDefinedByReviewer;
        }
    }
}
