using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AuthorBriefInfo
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public bool IsPrimaryContact { get; set; }
        public Guid ParticipantId { get; set; }
    }
}
