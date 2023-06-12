using System.Collections.Generic;
using System;
using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionReviewerAssignmentSuggestionDto
    {
        public Guid TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public Guid SubmissionId { get; set; }
        public string SubmissionTitle { get; set; } = string.Empty;
        public List<SubmissionSubjectAreaBriefInfo> SubmissionSubjectAreas { get; set; } = new List<SubmissionSubjectAreaBriefInfo>();
        public PagedResultDto<ReviewerWithFacts>? Reviewers { get; set; }
    }
}
