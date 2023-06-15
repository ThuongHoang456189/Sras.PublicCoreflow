using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class RegistrationPaper : FullAuditedAggregateRoot<Guid>
    {
        public Guid SubmissionId { get; private set; }
        public Submission Submission { get; private set; }
        public Guid RegistrationId { get; private set; }
        public Registration Registration { get; private set; }
        public int NumberOfPages { get; private set; }
        public int NumberOfExtraPages { get; private set; }
        public Guid? MainRegistrationPaperId { get; private set; }
        public RegistrationPaper? MainRegistrationPaper { get; private set; }
        public string? RootPresentationFilePath { get; set; }

        public RegistrationPaper(Guid id, Guid submissionId, 
            Guid registrationId, int numberOfPages, int numberOfExtraPages, 
            Guid? mainRegistrationPaperId, string? rootPresentationFilePath) : base(id)
        {
            SubmissionId = submissionId;
            RegistrationId = registrationId;
            NumberOfPages = numberOfPages;
            NumberOfExtraPages = numberOfExtraPages;
            MainRegistrationPaperId = mainRegistrationPaperId;
            RootPresentationFilePath = rootPresentationFilePath;
        }
    }
}
