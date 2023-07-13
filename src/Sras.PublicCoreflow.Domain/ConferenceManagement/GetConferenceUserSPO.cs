using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class GetConferenceUserSPO
    {
        public int? TotalCount { get; set; }
        public Guid? ConferenceAccountId { get; set; }
        public Guid? AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Organization { get; set; }
        public string? Country { get; set; }
        public string? SelectedRoles { get; set; }
    }
}