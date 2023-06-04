using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class UserConferenceRoleInput
    {
        public Guid AccountId { get; set; }
        public Guid ConferenceId { get; set; }
        public Guid? TrackId { get; set; }
        public List<RoleTrackPair> Roles { get; set; }
    }
}
