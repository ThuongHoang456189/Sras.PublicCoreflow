using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerConflictOperation
    {
        public Guid SubmissionId { get; set; }
        public Guid IncumbentId { get; set; }
        public Guid ConflictCaseId { get; set; }
        public bool IsDefinedByReviewer { get; set; }
        public ReviewerConflictManipulationOperators Operation { get; set; } = ReviewerConflictManipulationOperators.None;
    }
}
