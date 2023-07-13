using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerSubmissionAggregationDto
    {
        public Guid? PaperId { get; set; }
        public string? Title { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public List<AggregationSubjectAreaDto>? SubjectAreas { get; set; }
        public Guid? ReviewAssignmentId { get; set; }
        public List<string>? Actions { get; set; }
        public ReviewingSubmissionRelatedFilesDto? Files { get; set; }
    }
}
