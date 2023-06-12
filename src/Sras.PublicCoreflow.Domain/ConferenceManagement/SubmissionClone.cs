using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionClone : FullAuditedAggregateRoot<Guid>
    {
        public Guid SubmissionId { get; private set; }
        public Submission Submission { get; private set; }
        public bool IsLast { get; set; }
        public int CloneNo { get; private set; }

        public ICollection<ReviewAssignment> Reviews { get; set; }
        public ICollection<Revision> Revisions { get; set; }

        public SubmissionClone(Guid id, Guid submissionId, bool isLast, int cloneNo) : base(id)
        {
            SubmissionId = submissionId;
            IsLast = isLast;
            CloneNo = cloneNo;

            Reviews = new Collection<ReviewAssignment>();
            Revisions = new Collection<Revision>();
        }
    }
}
