using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class QuestionGroup : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }

        public ICollection<Question> Questions { get; private set; }

        public QuestionGroup(Guid id, string name) : base(id)
        {
            Name = name;

            Questions = new Collection<Question>();
        }

        public class DefaultQuestionGroups
        {
            public static QuestionGroup SubmissionQuestionGroup
                = new QuestionGroup(Guid.NewGuid(), "Submission Questions");
            public static QuestionGroup DecisionChecklistGroup
                = new QuestionGroup(Guid.NewGuid(), "Decision Checklist");
            public static QuestionGroup CameraReadyChecklistGroup
                = new QuestionGroup(Guid.NewGuid(), "Decision Checklist");
        }
    }
}
