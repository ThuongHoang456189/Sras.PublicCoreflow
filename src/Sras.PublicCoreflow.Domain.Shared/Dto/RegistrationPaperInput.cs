using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class RegistrationPaperInput
    {
        public Guid SubmissionId { get; set; }
        public int NumberOfPages { get; set; } = 0;
        public int? NumberOfExtraPages { get; set; } = 0;
    }
}
