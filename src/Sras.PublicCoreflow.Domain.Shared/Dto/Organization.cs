using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Organization
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string? OrganizationDescription { get; set;}
        public string? OrganizationWebsite { get; set;}
        public string OrganizationPhoneNumber { get; set; } //--internal phone number format
        public string? GRID { get; set; }
    }
}
