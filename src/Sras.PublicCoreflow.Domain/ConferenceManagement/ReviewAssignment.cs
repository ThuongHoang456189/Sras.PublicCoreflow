using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewAssignment : FullAuditedAggregateRoot<Guid>
    {
        public Guid SubmissionCloneId { get; private set; }
        public SubmissionClone SubmissionClone { get; private set; }
        public Guid ConferenceAccountId { get; private set; }
        public ConferenceReviewer ConferenceReviewer { get; private set; }
        public string? Review { get; private set; }
        public int? TotalScore { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsNotified { get; private set; }

        public ReviewAssignment(Guid id, Guid submissionCloneId, Guid conferenceAccountId, string? review, int? totalScore, bool isActive, bool isNotified) : base(id)
        {
            SubmissionCloneId = submissionCloneId;
            ConferenceAccountId = conferenceAccountId;
            Review = review;
            TotalScore = totalScore;
            IsActive = isActive;
            IsNotified = isNotified;
        }
    }
}
