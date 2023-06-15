using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Revision : FullAuditedAggregateRoot<Guid>
    {
        [ForeignKey(nameof(SubmissionClone))]
        public override Guid Id { get; protected set; }
        public SubmissionClone SubmissionClone { get; set; }
        public string? RootFilePath { get; set; }
        public Guid? PreviousRevisionId { get; private set; }
        public Revision? PreviousRevision { get; private set; }

        public Revision(Guid id, string? rootFilePath, Guid? previousRevisionId) : base(id)
        {
            RootFilePath = rootFilePath;
            PreviousRevisionId = previousRevisionId;
        }
    }
}
