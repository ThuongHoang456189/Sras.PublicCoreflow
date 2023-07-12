using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Education
    {
        public Guid educationId { get; set; }
        public int academicDegreeId { get; set; }// - Bac dao tao academic-degree-level-reference-types json
        public Organization educationalOrganization { get; set; } // - Noi dao tao
        public int startYear { get; set; } // Nam bat dau
        public int yearOfGraduation { get; set; } // Nam tot nghiep
        public string degreeAbbreviation { get; set; }
        public string degree { get; set; }
    }
}
