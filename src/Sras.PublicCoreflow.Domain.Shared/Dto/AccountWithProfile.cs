using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class AccountWithProfile
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Organization { get; set; }
        //public string? country { get; set; }
        public List<string> roles { get; set; }
        //public Object? googleScholarIdEntered { get; set; }
        //public Object? semanticScholarIdEntered { get; set; }
        //public Object? DblpIdEntered { get; set; }
        //public Object? openReviewIdEntered { get; set; }
        //public bool domainConflictsEntered { get; set; }
        //public bool individualConflictsEntered { get; set; }
        //public bool individualConflictsAttested { get; set; }
        //public DateTime lastVisit { get; set; }
    }
}