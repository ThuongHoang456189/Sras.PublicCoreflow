using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class QuestionGroupTrack : FullAuditedAggregateRoot<Guid>
    {
        public Guid QuestionGroupId { get; private set; }
        public QuestionGroup QuestionGroup { get; private set; }
        public Guid TrackId { get; private set; }
        public Track Track { get; private set; }
        public string Settings { get; private set; }

        public ICollection<Question> Questions { get; private set; }

        public QuestionGroupTrack(Guid id, Guid questionGroupId, Guid trackId, string settings) : base(id)
        {
            QuestionGroupId = questionGroupId;
            TrackId = trackId;
            Settings = settings;

            Questions = new Collection<Question>();
        }
    }
}
