using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class GuidelineGroupDto
    {
        public string? GuidelineGroup { get; set; }
        public List<TrackGuidelineDto>? Guidelines { get; set; }
    }
}
