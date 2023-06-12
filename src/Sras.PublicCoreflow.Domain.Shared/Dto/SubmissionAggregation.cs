using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionAggregation
    {
        public Guid SubmissionId { get; set; }
        public string? SubmissionTitle { get; set; }
        public List<AuthorBriefInfo>? Authors { get; set; }
        public int NumberOfUnregisteredAuthors { get; set; }
        public int NumberOfSubmissionFiles { get; set; }
        public List<SelectedSubjectAreaBriefInfo>? SubmissionSubjectAreas { get; set; }
        public TrackBriefInfo? Track { get; set; }
        public Guid? ChairNoteId { get; set; }
        public int NumberOfConflicts { get; set; }
        public int NumberOfDisputedConflicts { get; set; }
        public List<ReviewerBriefInfo>? Reviewers { get; set; }
        public int NumberOfAssignment { get; set; }
        public int NumberOfCompletedReviews { get; set; }
        public string? Status { get; set; }
        public bool IsRevisionSubmitted { get; set; }
        public bool IsRequestedForCameraReady { get; set; }
        public bool IsCameraReadySubmitted { get; set; }
        public bool IsRequestedForPresentation { get; set; }
    }
}
