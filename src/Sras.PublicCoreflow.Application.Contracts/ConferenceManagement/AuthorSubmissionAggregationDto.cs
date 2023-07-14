using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AuthorSubmissionAggregationDto
    {
        public Guid? SubmissionId { get; set; }
        public string? Title { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public SubmissionRelatedFilesDto? Files { get; set; }
        public Guid? StatusId { get; set; }
        public string? Status { get; set; }
        public List<string>? Actions { get; set; }
    }
}
