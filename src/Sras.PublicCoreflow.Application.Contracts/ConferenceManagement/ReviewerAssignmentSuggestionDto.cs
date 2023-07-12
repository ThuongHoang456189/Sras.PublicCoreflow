using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerAssignmentSuggestionDto
    {
        public Guid? ReviewerId { get; set; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Organization { get; set; }
        public List<string>? SubmissionConflicts { get; set; }
        public List<string>? ReviewerConflicts { get; set; }
        public List<AggregationSubjectAreaDto>? ReviewerSubjectAreas { get; set; }
        public int? Quota { get; set; }
        public bool? IsAssigned { get; set; }
        public int? NumberOfAssignments { get; set; }
        public double? Relevance { get; set; } // process
        public double? SortingFactor { get; set; } // process
    }
}
