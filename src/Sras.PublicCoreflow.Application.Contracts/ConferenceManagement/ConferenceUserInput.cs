using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceUserInput
    {
        public string? InclusionText { get; set; }
        public Guid? TrackId { get; set; }
        public Guid? ConferenceRoleId { get; set; }
        public int? SkipCount { get; set; }
        public int? MaxResultCount { get; set; }
    }
}
