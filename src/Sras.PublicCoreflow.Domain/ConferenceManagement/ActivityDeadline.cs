using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ActivityDeadline : FullAuditedAggregateRoot<Guid>
    {
        public Guid TrackId { get; private set; }
        public Track Track { get; private set; }
        public string Phase { get; private set; }
        public string Name { get; private set; }
        public DateTime? PlanDeadline { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsNext { get; set; }
        public byte Status { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string? GuidelineGroup { get; set; }
        public bool IsGuidelineShowed { get; set; }
        public int Factor { get; set; }
        public bool IsBeginPhaseMark { get; set; }
        public bool CanSkip { get; set; }
        public int? RevisionNo { get; set; }

        public ActivityDeadline(Guid id, Guid trackId,
            string phase, string name,
            DateTime? planDeadline, DateTime? deadline,
            bool isCurrent, bool isNext,
            byte status, DateTime? completionTime,
            string? guidelineGroup, bool isGuidelineShowed,
            int factor, bool isBeginPhaseMark, bool canSkip, int? revisionNo) : base(id)
        {
            TrackId = trackId;
            Phase = phase;
            Name = name;
            PlanDeadline = planDeadline;
            Deadline = deadline;
            IsCurrent = isCurrent;
            IsNext = isNext;
            Status = status;
            CompletionTime = completionTime;
            GuidelineGroup = guidelineGroup;
            IsGuidelineShowed = isGuidelineShowed;
            Factor = factor;
            IsBeginPhaseMark = isBeginPhaseMark;
            CanSkip = canSkip;
            RevisionNo = revisionNo;
        }
    }
}
