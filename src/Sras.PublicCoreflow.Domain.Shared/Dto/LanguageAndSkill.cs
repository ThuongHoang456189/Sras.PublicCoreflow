using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class LanguageAndSkill
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Result { get; set; }
        public string? Issuer { get; set; }
        public int? IssuedYear { get; set; }
    }
}
