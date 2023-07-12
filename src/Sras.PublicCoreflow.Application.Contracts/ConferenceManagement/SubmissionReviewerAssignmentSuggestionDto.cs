using System.Collections.Generic;
using System;
using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionReviewerAssignmentSuggestionDto
    {
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public Guid? PaperId { get; set; }
        public string? Title { get; set; }
        public List<AggregationSubjectAreaDto>? SubmissionSubjectAreas { get; set; }
        public PagedResultDto<ReviewerAssignmentSuggestionDto>? Reviewers { get; set; }
    }
}
