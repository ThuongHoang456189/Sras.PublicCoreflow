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

        public ICollection<ReviewAssignment> Reviews { get; private set; }

        public SubmissionClone(Guid id, Guid submissionId) : base(id)
        {
            SubmissionId = submissionId;

            Reviews = new Collection<ReviewAssignment>();
        }
    }
}
