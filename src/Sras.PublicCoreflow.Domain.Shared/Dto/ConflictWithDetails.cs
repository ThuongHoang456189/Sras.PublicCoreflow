using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConflictWithDetails
    {
        public Guid SubmissionId { get; set; }
        public Guid IncumbentId { get; set; }
        public Guid ConflictCaseId { get; set; }
        public string ConflictCaseName { get; set; }
        public bool IsIndividualConflictCase { get; set; }
        public bool IsDefaultConflictCase { get; set; }
        public Guid? TrackId { get; set; }
        public bool IsDefinedByReviewer { get; set; }
    }
}
