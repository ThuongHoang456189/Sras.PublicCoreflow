using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AuthorInput
    {
        public Guid? ParticipantId { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Organization { get; set; }
        public string? Country { get; set; }
    }
}
