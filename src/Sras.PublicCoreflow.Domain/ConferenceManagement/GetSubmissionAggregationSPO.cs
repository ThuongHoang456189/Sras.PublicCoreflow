using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class GetSubmissionAggregationSPO
    {
        public int? TotalCount { get; set; }
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public string? SelectedAuthors { get; set; }
        public string? SelectedSubmissionSubjectAreas { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public int? SubmissionConflicts { get; set; }
        public int? ReviewerConflicts { get; set; }
        public int? Assigned { get; set; }
        public int? Reviewed { get; set; }
        public int? AverageScore { get; set; }
        public Guid? StatusId { get; set; }
        public string? Status { get; set; }
        public Guid? LatestSubmissionCloneId { get; set; }
        public int? CloneNo { get; set; }
        public bool? IsRequestedForCameraReady { get; set; }
        public Guid? CameraReadyId { get; set; }
        public bool? IsRequestedForPresentation { get; set; }
    }
}
