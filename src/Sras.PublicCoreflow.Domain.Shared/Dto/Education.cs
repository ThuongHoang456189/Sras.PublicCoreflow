using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Education
    {
        public Guid EducationId { get; set; }
        public Guid AcademicDegreeId { get; set; }// - Bac dao tao academic-degree-level-reference-types json
        public Organization EducationalOrganization { get; set; } // - Noi dao tao
        public int StartYear { get; set; } // Nam bat dau
        public int YearOfGraduation { get; set; } // Nam tot nghiep
        public string DegreeAbbreviation { get; set; }
        public string Degree { get; set; }
    }
}
