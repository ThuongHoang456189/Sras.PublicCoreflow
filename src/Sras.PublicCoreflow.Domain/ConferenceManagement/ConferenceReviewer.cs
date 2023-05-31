using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceReviewer : FullAuditedAggregateRoot<Guid>
    {
        [ForeignKey(nameof(ConferenceAccount))]
        public Guid Id { get; private set; }
        public virtual ConferenceAccount ConferenceAccount { get; set; }
        public int? Quota { get; private set; }

        public ICollection<ConferenceReviewerSubjectArea> SubjectAreas { get; private set; }
        public ICollection<ReviewAssignment> Reviews { get; private set; }

        public ConferenceReviewer(Guid id, int? quota) :base (id)
        {
            Quota = quota;

            SubjectAreas = new Collection<ConferenceReviewerSubjectArea>();
            Reviews = new Collection<ReviewAssignment>();
        }
    }
}
