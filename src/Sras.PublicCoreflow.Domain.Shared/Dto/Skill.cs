using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Skill
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public string? result { get; set; }
        public string? issuer { get; set; }
        public int? issuedYear { get; set; }
    }
}
