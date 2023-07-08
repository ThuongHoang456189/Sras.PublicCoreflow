using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionAttachment : FullAuditedAggregateRoot<Guid>
    {
        [ForeignKey(nameof(Submission))]
        public override Guid Id { get; protected set; }
        public Submission Submission { get; protected set; }
        public string? RootSupplementaryMaterialFilePath { get; set; }
        public string? RootPresentationFilePath { get; set; }

        public SubmissionAttachment(Guid id, string? rootSupplementaryMaterialFilePath, string? rootPresentationFilePath) : base(id)
        {
            RootSupplementaryMaterialFilePath = rootSupplementaryMaterialFilePath;
            RootPresentationFilePath = rootPresentationFilePath;
        }
    }
}
