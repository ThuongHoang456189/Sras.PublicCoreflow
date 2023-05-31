using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Author : FullAuditedAggregateRoot
    {
        [ForeignKey(nameof(Participant))]
        public Guid ParticipantId { get; private set; }
        public Participant Participant { get; private set; }
        [ForeignKey(nameof(Submission))]
        public Guid SubmissionId { get; private set; }
        public Submission Submission { get; private set; }
        public bool IsPrimaryContact { get; private set; }
        public bool IsPresenter { get; private set; }

        public Author(Guid participantId, Guid submissionId, bool isPrimaryContact, bool isPresenter)
        {
            ParticipantId = participantId;
            SubmissionId = submissionId;
            IsPrimaryContact = isPrimaryContact;
            IsPresenter = isPresenter;
        }

        public override object[] GetKeys()
        {
            return new object[] { ParticipantId, SubmissionId };
        }
    }
}
