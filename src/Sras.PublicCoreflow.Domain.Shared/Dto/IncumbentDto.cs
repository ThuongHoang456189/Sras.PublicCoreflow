using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class IncumbentDto
    {
        public Guid ConferenceAccountId { get; set; }
        public Guid ConferenceRoleId { get; set; }
        public Guid? TrackId { get; set; }
        public bool IsPrimaryContact { get; set; }
    }
}
