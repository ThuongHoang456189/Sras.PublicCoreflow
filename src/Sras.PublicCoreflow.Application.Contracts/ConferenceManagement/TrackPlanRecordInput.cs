using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class TrackPlanRecordInput
    {
        public Guid Id { get; set; }
        public Guid TrackId { get; set; }
        public string Phase { get; set; }
        public string Name { get; set; }
        public DateTime? PlanDeadline { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsNext { get; set; }
        public string Status { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string? GuidelineGroup { get; set; }
        public bool IsGuidelineShowed { get; set; }
        public int Factor { get; set; }
        public bool IsBeginPhaseMark { get; set; }
        public bool CanSkip { get; set; }
        public int? RevisionNo { get; set; }
    }
}
