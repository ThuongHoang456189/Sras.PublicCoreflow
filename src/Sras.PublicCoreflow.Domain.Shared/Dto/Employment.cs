using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Employment
    {
        public Guid EmploymentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EmploymentPosition { get; set; }
        public Organization Organization { get; set; }
    }
}
