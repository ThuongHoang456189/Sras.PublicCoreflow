using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionSummarySPO
    {
        public string? ConferenceFullName { get; set; }
        public string? ConferenceShortName { get; set; }
        public string? TrackName { get; set; }
        public Guid? PaperId { get; set; }
        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string? SelectedAuthors { get; set; }
        public string? SelectedSubmissionSubjectAreas { get; set; }
        public string? DomainConflicts { get; set; }
        public string? SelectedSubmissionConflictedIncumbents { get; set; }
        public string? SubmissionRootFilePath { get; set; }
        public string? SupplementaryMaterialRootFilePath { get; set; }
        public string? SubmittedRevisionNo { get; set; }
        public string? RevisionRootFilePath { get; set; }
        public string? CameraReadyRootFilePath { get; set; }
        public string? CopyRightFilePath { get; set; }
        public string? PresentationRootFilePath { get; set; }
        public string? SubmissionQuestionsResponse { get; set; }
    }
}
