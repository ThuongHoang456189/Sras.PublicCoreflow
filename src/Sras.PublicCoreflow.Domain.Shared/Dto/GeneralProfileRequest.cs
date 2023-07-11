using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class GeneralProfileRequest
    {
        public Guid userId { get; set; }
        public string publishName { get; set; }
        public string primaryEmail { get; set; }
        public string? orcid { get; set; } // json -- by ReferenceWithReferenceTypeInclusion
        public string? introduction { get; set; }
        public DateTime dateOfBirth { get; set; } //-just date only remove time
        public string gender { get; set; }
        public string? scientistTitle { get; set; }
        public string? adminPositionFunction { get; set; }
        public string? academicFunction { get; set; }
        public int? academic { get; set; }
        public string? currentDegree { get; set; }
        public int? current { get; set; }
        public string? homeAddress { get; set; }
        public string? phoneNumber { get; set; }
        public string? mobilePhone { get; set; }
        public string? fax { get; set; }
        // 17 field
        public string? otherIds { get; set; }
        public string? websiteAndSocialLinks { get; set; }
        public string? alsoKnownAs { get; set; }
    }
}
