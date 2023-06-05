using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceWithDetailsInput
    {
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? WebsiteLink { get; set; }
        public string Logo { get; set; }
        public bool IsSingleTrack { get; set; }
        // List of chair
        public List<Guid> Chairs { get; set; } = new List<Guid>();
        public List<string>? Tracks { get; set; }
    }
}
