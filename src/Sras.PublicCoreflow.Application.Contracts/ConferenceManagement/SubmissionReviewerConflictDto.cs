using System;
using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionReviewerConflictDto
    {
        public Guid SubmissionId { get; set; }
        public string? SubmissionTitle { get; set; }
        public Guid TrackId { get; set; }
        public string? TrackName { get; set; }
        public PagedResultDto<ReviewerWithConflictDetails>? Reviewers { get; set; }
    }
}
