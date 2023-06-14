using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerWithFacts
    {
        public Guid ReviewerId { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Organization { get; set; }
        public List<ConflictWithDetails> SubmissionConflicts { get; set; }
        public List<ConflictWithDetails> ReviewerConflicts { get; set; }
        public List<ReviewerSubjectAreaBriefInfo> ReviewerSubjectAreas { get; set; }
        public double Relevance { get; set; }
        public int? Quota { get; set; }
        public int NumberOfAssignments { get; set; }
        public bool IsAssigned { get; set; }
        public double SortingFactor { get; set; }
    }
}
