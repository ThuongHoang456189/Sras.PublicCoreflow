using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Reviewer : FullAuditedAggregateRoot<Guid>
    {
        [ForeignKey(nameof(Incumbent))]
        public Guid Id { get; private set; }
        public Incumbent Incumbent { get; set; }
        public int? Quota { get; private set; }

        public ICollection<ReviewerSubjectArea> SubjectAreas { get; private set; }
        public ICollection<ReviewAssignment> Reviews { get; private set; }

        public Reviewer(Guid id, int? quota) :base (id)
        {
            Quota = quota;

            SubjectAreas = new Collection<ReviewerSubjectArea>();
            Reviews = new Collection<ReviewAssignment>();
        }
    }
}
