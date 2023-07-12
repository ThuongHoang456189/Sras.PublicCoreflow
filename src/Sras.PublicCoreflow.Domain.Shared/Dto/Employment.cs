using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Employment
    {
        public Guid employmentId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string employmentPosition { get; set; }
        public Organization organization { get; set; }
    }
}
