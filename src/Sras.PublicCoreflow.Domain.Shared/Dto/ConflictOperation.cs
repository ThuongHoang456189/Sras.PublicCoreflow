using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConflictOperation
    {
        public Guid SubmissionId { get; set; }
        public Guid IncumbentId { get; set; }
        public Guid ConflictCaseId { get; set; }
        public bool IsDefinedByReviewer { get; set; }
        public ConflictManipulationOperators Operation { get; set; } = ConflictManipulationOperators.None;
    }
}
