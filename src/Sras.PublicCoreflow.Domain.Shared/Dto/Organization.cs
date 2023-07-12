using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Organization
    {
        public Guid organizationId { get; set; }
        public string organizationName { get; set; }
        public string? organizationDescription { get; set;}
        public string? organizationWebsite { get; set;}
        public string organizationPhoneNumber { get; set; } //--internal phone number format
        public string? grid { get; set; }
    }
}
