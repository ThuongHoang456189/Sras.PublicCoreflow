using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerSubjectAreaOperation : SelectedSubjectAreaInput
    {
        public Guid ReviewerId { get; set; }
        public ReviewerSubjectAreaManipulationOperators Operation { get; set; } = ReviewerSubjectAreaManipulationOperators.None;
    }
}
