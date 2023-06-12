using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Submission : FullAuditedAggregateRoot<Guid>
    {
        public string Title { get; private set; }
        public string Abstract { get; private set; }
        public string? RootFilePath { get; private set; }
        public Guid TrackId { get; private set; }
        public Track Track { get; private set; }
        public string? DomainConflicts { get; private set; }
        public Guid? CreatedIncumbentId { get; private set; }
        public Incumbent? CreatedIncumbent { get; private set; }
        public Guid? LastModifiedIncumbentId { get; private set; }
        public Incumbent? LastModifiedIncumbent { get; private set; }
        public string? Answers { get; private set; }
        public Guid StatusId { get; set; }
        public PaperStatus Status { get; private set; }
        public bool? IsNotified { get; set; }
        public Guid? NotifiedStatusId { get; set; }
        public PaperStatus? NotifiedStatus { get; private set; }
        public DateTime? LastNotificationTime { get; private set; }
        public bool IsFinallyDecided { get; private set; }
        public bool IsRequestedForCameraReady { get; private set; }
        public DateTime? CameraReadyRequestTime { get; private set; }
        public bool IsRequestedForPresentation { get; private set; }
        public DateTime? PresentationRequestTime { get; private set; }

        public ICollection<SubmissionSubjectArea> SubjectAreas { get; set; }
        public ICollection<Author> Authors { get; set; }
        public ICollection<Conflict> Conflicts { get; set; }
        public ICollection<SubmissionClone> Clones { get; set; }

        public Submission(Guid id, string title, 
            string @abstract, string? rootFilePath, 
            Guid trackId, string? domainConflicts, 
            Guid? createdIncumbentId, Guid? lastModifiedIncumbentId, 
            string? answers, Guid statusId, bool? isNotified, 
            Guid? notifiedStatusId, DateTime? lastNotificationTime,
            bool isFinallyDecided, 
            bool isRequestedForCameraReady, DateTime? cameraReadyRequestTime,
            bool isRequestedForPresentation, DateTime? presentationRequestTime) : base(id)
        {
            SetTitle(title);
            SetAbstract(@abstract);
            RootFilePath = rootFilePath;
            TrackId = trackId;
            DomainConflicts = domainConflicts;
            CreatedIncumbentId = createdIncumbentId;
            LastModifiedIncumbentId = lastModifiedIncumbentId;
            Answers = answers;
            StatusId = statusId;
            IsNotified = isNotified;
            NotifiedStatusId = notifiedStatusId;
            LastNotificationTime = lastNotificationTime;
            IsFinallyDecided = isFinallyDecided;
            IsRequestedForCameraReady = isRequestedForCameraReady;
            CameraReadyRequestTime = cameraReadyRequestTime;
            IsRequestedForPresentation = isRequestedForPresentation;
            PresentationRequestTime = presentationRequestTime;

            SubjectAreas = new Collection<SubmissionSubjectArea>();
            Authors = new Collection<Author>();
            Conflicts = new Collection<Conflict>();
            Clones = new Collection<SubmissionClone>();
        }

        public Submission SetTitle(string title)
        {
            Title = Check.NotNullOrWhiteSpace(string.IsNullOrEmpty(title) ? title : title.Trim(), nameof(title), SubmissionConsts.MaxTitleLength);
            return this;
        }

        public Submission SetAbstract(string @abstract)
        {
            Abstract = Check.NotNullOrWhiteSpace(string.IsNullOrEmpty(@abstract) ? @abstract : @abstract.Trim(), nameof(@abstract), SubmissionConsts.MaxAbstractLength);
            return this;
        }

        public Submission AddSubmissionSubjectArea(Guid submissionSubjectAreaId, Guid subjectAreaId, bool isPrimary)
        {
            if(SubjectAreas.Any(x => x.SubjectAreaId == subjectAreaId))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubjectAreaAlreadyExistToSubmission);
            }

            SubjectAreas.Add(new SubmissionSubjectArea(submissionSubjectAreaId, Id, subjectAreaId, isPrimary));

            return this;
        }

        public Submission AddAuthor(Guid authorId, Guid participantId, bool isPrimaryContact)
        {
            if(Authors.Any(x => x.ParticipantId == participantId))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.AuthorAlreadyExistToSubmission);
            }

            Authors.Add(new Author(authorId, participantId, Id, isPrimaryContact, false));

            return this;
        }
    }
}
