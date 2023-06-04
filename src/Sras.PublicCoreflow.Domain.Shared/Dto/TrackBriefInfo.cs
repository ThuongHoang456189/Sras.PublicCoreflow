using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class TrackBriefInfo
    {
        public Guid TrackId { get; set; }
        public string TrackName { get; set; }
        public bool IsDefault { get; set; }

        public TrackBriefInfo(Guid trackId, string trackName)
        {
            TrackId = trackId;
            TrackName = trackName;
        }
    }
}
