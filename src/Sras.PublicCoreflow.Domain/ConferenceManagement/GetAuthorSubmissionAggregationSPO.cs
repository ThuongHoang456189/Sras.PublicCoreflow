using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class GetAuthorSubmissionAggregationSPO
    {
        public int? TotalCount { get; set; }
        // Submission Id
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public string? SubmissionRootFilePath { get; set; }
        public string? SupplementaryMaterialRootFilePath { get; set; }
        public int? CloneNo { get; set; }
        public string? RevisionRootFilePath { get; set; }
        public string? CameraReadyRootFilePath { get; set; }
        public string? CopyRightFilePath { get; set; }
        public string? PresentationRootFilePath { get; set; }
        public string? DeadlineName { get; set; }
        public Guid? StatusId { get; set; }
        public string? Status { get; set; }
        public string? Actions { get; set; }
    }
}
