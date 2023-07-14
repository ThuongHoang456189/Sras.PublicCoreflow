using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class GetReviewerSubmissionAggregationSPO
    {
        public int? TotalCount { get; set; }
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public string? SelectedSubmissionSubjectAreas { get; set; }
        public Guid? ReviewAssignmentId { get; set; }
        public string? Actions { get; set; }
        public string? SubmissionRootFilePath { get; set; }
        public string? SupplementaryMaterialRootFilePath { get; set; }
        public string? RevisionRootFilePath { get; set; }
        public int? CloneNo { get; set; }
    }
}
