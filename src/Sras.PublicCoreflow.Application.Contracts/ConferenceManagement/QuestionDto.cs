using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public Guid QuestionGroupId { get; set; }
        public Guid TrackId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsRequired { get; set; }
        public bool? IsVisibleToReviewers { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
        public ShowAsDto? ShowAs { get; set; }
    }
}
