using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceUserDto
    {
        public Guid? ConferenceAccountId { get; set; }
        public Guid? AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Organization { get; set; }
        public string? Country { get; set; }
        public List<string>? Roles { get; set; }
    }
}
