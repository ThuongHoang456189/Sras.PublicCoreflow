namespace Sras.PublicCoreflow.ConferenceManagement
{
    public static class ActivityDeadlineConsts
    {
        public const string StartDate = "Start Date";
        public const string CallForPapersDeadline = "Call For Papers Deadline";
        public const string SubmissionDeadline = "Submission Deadline";
        public const string SubmissionEditsDeadline = "Submission Edits Deadline";
        public const string SupplementaryMaterialDeadline = "Supplementary Material Deadline";
        public const string ReviewSubmissionDeadline = "Review Submission Deadline";
        public const string RevisionNSubmissionDeadline = "Revision {n} Submission Deadline";
        public const string RevisionNReviewSubmissionDeadline = "Revision {n} Review Submission Deadline";
        public const string ResultNotificationDeadline = "Result Notification Deadline";
        public const string CameraReadySubmissionDeadline = "Camera Ready Submission Deadline";
        public const string PresentationSubmissionDeadline = "Presentation Submission Deadline";
        public const string EndDate = "End Date";

        public const string StartDatePhase = "New Beginning";
        public const string CallForPapersDeadlinePhase = "Calling For Papers";
        public const string SubmissionDeadlinePhase = "Open for Submission";
        public const string SubmissionEditsDeadlinePhase = "Open for Submission Edit";
        public const string SupplementaryMaterialDeadlinePhase = "Open for Supplementary Material";
        public const string ReviewSubmissionDeadlinePhase = "Awaiting Submission Review";
        public const string RevisionNSubmissionDeadlinePhase = "Open for Revision {n} Submission";
        public const string RevisionNReviewSubmissionDeadlinePhase = "Awaiting Revision {n} Review";
        public const string ResultNotificationDeadlinePhase = "Notifying Result";
        public const string CameraReadySubmissionDeadlinePhase = "Open for Camera Ready Submission";
        public const string PresentationSubmissionDeadlinePhase = "Open for Presentation Submission";
        public const string EndDatePhase = "In Progress & Completed";

        public const byte Disabled = 0;
        public const byte Enabled = 1;
        public const byte Completed = 2;

        public const string DisabledStatus = "Disabled";
        public const string EnabledStatus = "Enabled";
        public const string CompletedStatus = "Completed";

        public const int MaxPhaseLength = 64;
        public const int MaxNameLength = 64;
        public const int MaxGuidelineGroupLength = 64;

        public const string PreSubmissionGuidelineGroup = "Pre-Submission Guidelines";
        public const string PreReviewSubmissionGuidelineGroup = "Pre-Review-Submission Guidelines";
        public const string PreResultNotificationGuidelineGroup = "Pre-Result-Notification Guidelines";
        public const string PreCameraReadySubmissionGuidelineGroup = "Pre-Camera-Ready-Submission Guidelines";
        public const string PrePresentationSubmissionGuidelineGroup = "Pre-Presentation-Submission Guidelines";
    }
}