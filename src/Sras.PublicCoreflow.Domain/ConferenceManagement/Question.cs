using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Question : FullAuditedAggregateRoot<Guid>
    {
        public Guid QuestionGroupId { get; private set; }
        public QuestionGroup QuestionGroup { get; private set; }
        public Guid TrackId { get; private set; }
        public Track Track { get; private set; }
        public string Title { get; set; }
        public string? Text { get; set; }
        public bool IsRequired { get; set; }
        public bool? IsVisibleToReviewers { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
        public string ShowAs { get; set; }
        public int Index { get; set; }

        public Question(Guid id, Guid questionGroupId, Guid trackId, string title, 
            string? text, bool isRequired, bool? isVisibleToReviewers, 
            string type, string typeName, string showAs, int index) : base(id)
        {
            QuestionGroupId = questionGroupId;
            TrackId = trackId;
            Title = title;
            Text = text;
            IsRequired = isRequired;
            IsVisibleToReviewers = isVisibleToReviewers;
            Type = type;
            TypeName = typeName;
            ShowAs = showAs;
            Index = index;
        }
    }
}
