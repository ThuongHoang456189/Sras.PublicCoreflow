using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Author : FullAuditedAggregateRoot<Guid>
    {
        public Guid ParticipantId { get; private set; }
        public Participant Participant { get; private set; }
        public Guid SubmissionId { get; private set; }
        public Submission Submission { get; private set; }
        public bool IsPrimaryContact { get; private set; }
        public bool IsPresenter { get; private set; }

        public Author(Guid id, Guid participantId, Guid submissionId, bool isPrimaryContact, bool isPresenter) : base(id)
        {
            ParticipantId = participantId;
            SubmissionId = submissionId;
            IsPrimaryContact = isPrimaryContact;
            IsPresenter = isPresenter;
        }
    }
}
