using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Question : FullAuditedAggregateRoot<Guid>
    {
        public string Title { get; private set; }
        public string Text { get; private set; }
        public string Settings { get; private set; }
        public Guid QuestionGroupTrackId { get; private set; }
        public QuestionGroupTrack QuestionGroupTrack { get; private set; }
        public Guid? NextQuestionId { get; private set; }
        public Question? NextQuestion { get; private set; }

        public Question(Guid id, string title, string text, string settings, Guid questionGroupTrackId, Guid? nextQuestionId) : base(id) 
        {
            Title = title;
            Text = text;
            Settings = settings;
            QuestionGroupTrackId = questionGroupTrackId;
            NextQuestionId = nextQuestionId;
        }
    }
}
