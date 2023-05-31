using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class QuestionGroup : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string Settings { get; private set; }

        public ICollection<QuestionGroupTrack> Tracks { get; private set; }

        public QuestionGroup(Guid id, string name, string settings) : base(id)
        {
            Name = name;
            Settings = settings;

            Tracks = new Collection<QuestionGroupTrack>();
        }
    }
}
