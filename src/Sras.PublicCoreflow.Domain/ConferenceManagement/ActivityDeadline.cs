using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ActivityDeadline : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public byte Status { get; private set; }
        public DateTime? Deadline { get; private set; }
        public Guid TrackId { get; private set; }
        public Track Track { get; private set; }

        public ActivityDeadline(Guid id, string name, byte status, DateTime? deadline, Guid trackId) : base(id) 
        {
            Name = name;
            Status = status;
            Deadline = deadline;
            TrackId = trackId;
        }
    }
}
