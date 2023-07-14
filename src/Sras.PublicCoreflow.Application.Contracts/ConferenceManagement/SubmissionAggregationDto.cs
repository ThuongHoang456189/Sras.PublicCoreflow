using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionAggregationDto
    {
        public Guid? PaperId { get; set; }
        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public List<SubmissionAuthorDto>? Authors { get; set; }
        public List<AggregationSubjectAreaDto>? SubjectAreas { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public int? SubmissionConflicts { get; set; }
        public int? ReviewerConflicts { get; set; }
        public int? Assigned { get; set; }
        public int? Reviewed { get; set; }
        public int? AverageScore { get; set; }
        public Guid? StatusId { get; set; }
        public string? Status { get; set; }
        public bool? RevisionSubmitted { get; set; }
        public int? RevisionNo { get; set; }
        public bool? IsRequestedForCameraReady { get; set; }
        public bool? CameraReadySubmitted { get; set; }
        public bool? IsRequestedForPresentation { get; set; }
    }
}
