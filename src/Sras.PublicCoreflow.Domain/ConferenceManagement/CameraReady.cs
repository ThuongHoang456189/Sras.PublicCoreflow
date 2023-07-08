using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class CameraReady : FullAuditedAggregateRoot<Guid>
    {
        [ForeignKey(nameof(Submission))]
        public override Guid Id { get; protected set; }
        public Submission Submission { get; protected set; }
        public string? RootCameraReadyFilePath { get; set; }
        public string? CopyRightFilePath { get; private set; }

        public CameraReady(Guid id, string? rootCameraReadyFilePath, string? copyRightFilePath) : base(id)
        {
            RootCameraReadyFilePath = rootCameraReadyFilePath;
            CopyRightFilePath = copyRightFilePath;
        }
    }
}
