using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class RoleTrackOperation
    {
        public Guid ConferenceAccountId { get; set; }
        public Guid IncumbentId { get; set; }
        public Guid RoleId { get; set; }
        public Guid? TrackId { get; set; }
        public RoleTrackManipulationOperators Operation { get; set; } = RoleTrackManipulationOperators.None;

        public RoleTrackOperation(Guid conferenceAccountId, Guid incumbentId, Guid roleId, Guid? trackId, RoleTrackManipulationOperators operation)
        {
            ConferenceAccountId = conferenceAccountId;
            IncumbentId = incumbentId;
            RoleId = roleId;
            TrackId = trackId;
            Operation = operation;
        }
    }
}
