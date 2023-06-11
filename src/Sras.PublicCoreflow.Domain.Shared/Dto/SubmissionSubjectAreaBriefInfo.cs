using Sras.PublicCoreflow.Dto;
using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionSubjectAreaBriefInfo : SubjectAreaBriefInfo
    {
        public Guid SubmissionId { get; set; }
        public bool IsPrimary { get; set; }
    }
}
