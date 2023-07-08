using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ScholarshipAndAward
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Issuer { get; set; }
        public int? IssuedYear { get; set; }
    }
}
