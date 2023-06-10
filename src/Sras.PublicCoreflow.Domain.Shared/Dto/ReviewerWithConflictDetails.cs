using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerWithConflictDetails
    {
        public Guid ReviewerId { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Organization { get; set; }
        public List<ConflictWithDetails> Conflicts { get; set; }
    }
}
