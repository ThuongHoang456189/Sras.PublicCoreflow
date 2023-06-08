using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerSubjectAreaInput
    {
        public Guid AccountId { get; set; }
        public Guid ConferenceId { get; set; }
        public Guid TrackId { get; set; }
        public List<SelectedSubjectAreaInput> SubjectAreas { get; set; }
    }
}
