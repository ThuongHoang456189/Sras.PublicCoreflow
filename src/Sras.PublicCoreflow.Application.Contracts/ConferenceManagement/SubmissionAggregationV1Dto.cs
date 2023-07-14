using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionAggregationV1Dto
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Authors { get; set; }
        public string? SubjectAreas { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public int? SubmissionConflicts { get; set; }
        public int? ReviewerConflicts { get; set; }
        public int? Assigned { get; set; }
        public int? Reviewed { get; set; }
        public int? AverageScore { get; set; }
        public Guid? StatusId { get; set; }
        public string? Status { get; set; }
        public int? CloneNo { get; set; }
        public bool? IsRevisionSubmitted { get; set; }
        public bool? IsRequestedForCameraReady { get; set; }
        public bool? IsCameraReadySubmitted { get; set; }
        public bool? IsRequestedForPresentation { get; set; }
    }
}
