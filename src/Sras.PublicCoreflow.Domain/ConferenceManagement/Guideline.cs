using System;
using Volo.Abp.Domain.Entities;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Guideline : Entity<Guid>
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string GuidelineGroup { get; private set; }
        public bool IsChairOnly { get; private set; }
        public string? Route { get; private set; }
        public int Factor { get; private set; }

        public Guideline(Guid id, string name, string? description, string guidelineGroup, bool isChairOnly, string? route, int factor) : base(id)
        {
            Name = name;
            Description = description;
            GuidelineGroup = guidelineGroup;
            IsChairOnly = isChairOnly;
            Route = route;
            Factor = factor;
        }
    }
}
