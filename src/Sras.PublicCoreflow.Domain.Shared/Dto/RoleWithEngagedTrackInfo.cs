using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class RoleWithEngagedTrackInfo
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public int Factor { get; set; }
        public List<TrackBriefInfo>? EngagedTracks { get; set; }

        public RoleWithEngagedTrackInfo(Guid roleId, string roleName, int factor)
        {
            RoleId = roleId;
            RoleName = roleName;
            Factor = factor;
        }
    }
}
