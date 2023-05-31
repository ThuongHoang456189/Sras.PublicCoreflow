using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Submission : FullAuditedAggregateRoot<Guid>
    {
        public string Title { get; private set; }
        public string Abstract { get; private set; }
        public string RootFilePath { get; private set; }
        public Guid TrackId { get; private set; }
        public Track Track { get; private set; }
        public string DomainConflicts { get; private set; }
        public Guid CreatedIncumbentId { get; private set; }
        public Incumbent CreatedIncumbent { get; private set; }
        public Guid LastModifiedIncumbentId { get; private set; }
        public Incumbent LastModifiedIncumbent { get; private set; }
        public string Answers { get; private set; }
        public Guid StatusId { get; private set; }
        public PaperStatus Status { get; private set; }
        public bool IsNotified { get; private set; }
        public Guid NotifiedStatusId { get; private set; }
        public PaperStatus NotifiedStatus { get; private set; }
        public bool IsRequestedForCameraReady { get; private set; }

        public ICollection<SubmissionSubjectArea> SubjectAreas { get; private set; }
        public ICollection<Author> Authors { get; private set; }
        public ICollection<Conflict> Conflicts { get; private set; }
        public ICollection<SubmissionClone> Clones { get; private set; }

        public Submission(Guid id, string title, string @abstract, string rootFilePath, Guid trackId, string domainConflicts, Guid createdIncumbentId, Guid lastModifiedIncumbentId, string answers, Guid statusId, bool isNotified, Guid notifiedStatusId, bool isRequestedForCameraReady) : base(id)
        {
            Title = title;
            Abstract = @abstract;
            RootFilePath = rootFilePath;
            TrackId = trackId;
            DomainConflicts = domainConflicts;
            CreatedIncumbentId = createdIncumbentId;
            LastModifiedIncumbentId = lastModifiedIncumbentId;
            Answers = answers;
            StatusId = statusId;
            IsNotified = isNotified;
            NotifiedStatusId = notifiedStatusId;
            IsRequestedForCameraReady = isRequestedForCameraReady;

            SubjectAreas = new Collection<SubmissionSubjectArea>();
            Authors = new Collection<Author>();
            Conflicts = new Collection<Conflict>();
            Clones = new Collection<SubmissionClone>();
        }
    }
}
