using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AccountWithBriefInfo
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? Organization { get; set; }
        public string? Country { get; set; }
        public Guid? ParticipantId { get; set; }
    }
}
