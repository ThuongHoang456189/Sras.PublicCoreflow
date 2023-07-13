using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionSummaryDto
    {
        public string? ConferenceFullName { get; set; }
        public string? ConferenceShortName { get; set; }
        public string? TrackName { get; set; }
        public Guid? PaperId { get; set; }
        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public List<SubmissionAuthorDto>? Authors { get; set; }
        public List<AggregationSubjectAreaDto>? SubjectAreas { get; set; }
        public string? DomainConflicts { get; set; }
        public List<SubmissionSummarySubmissionConflictedIncumbentDto>? ConflictsOfInterest { get; set; }
        public List<string>? SubmissionFiles { get; set; }
        public List<string>? SupplementaryMaterialFiles { get; set; }
        public string? SubmittedRevisionNo { get; set; } 
        public List<string>? RevisionFiles { get; set; }
        public List<string>? CameraReadyFiles { get; set; }
        public List<string>? CopyRightFiles { get; set; }
        public List<string>? PresentationFiles { get; set; }
        public string? SubmissionQuestionsResponse { get; set; }
    }
}
