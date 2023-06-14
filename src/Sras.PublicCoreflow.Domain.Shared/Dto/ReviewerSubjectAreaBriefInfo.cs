using Sras.PublicCoreflow.Dto;
using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerSubjectAreaBriefInfo : SubjectAreaBriefInfo
    {
        public Guid ReviewerId { get; set; }
        public bool IsPrimary { get; set; }
    }
}
