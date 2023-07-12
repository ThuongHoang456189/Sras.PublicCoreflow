using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ScholarshipAndAward
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public string? issuer { get; set; }
        public int? issuedYear { get; set; }
    }
}
