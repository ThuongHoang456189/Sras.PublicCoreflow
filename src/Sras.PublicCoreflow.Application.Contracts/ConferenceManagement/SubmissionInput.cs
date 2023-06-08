using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionInput
    {
        public Guid TrackId { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public List<AuthorInput> Authors { get; set; }
        public string DomainConflicts { get; set; }
        public List<SelectedSubjectAreaInput> SubjectAreas { get; set; }
        //public List<RemoteStreamContent> Files { get; set; }
        public string? Answers { get; set; }
    }
}
