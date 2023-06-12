using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SelectedSubjectAreaBriefInfo
    {
        public Guid SubjectAreaId { get; set; }
        public string? SubjectAreaName { get; set; }
        public bool IsPrimary { get; set; }
    }
}
