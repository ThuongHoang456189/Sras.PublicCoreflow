namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class TrackGuidelineDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string GuidelineGroup { get; set; }
        public bool IsChairOnly { get; set; }
        public string? Route { get; set; }
        public int Factor { get; set; }
    }
}
