using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Sras.PublicCoreflow.Extension;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Track : FullAuditedAggregateRoot<Guid>
    {
        public bool IsDefault { get; private set; }
        public string Name { get; private set; }
        public Guid ConferenceId { get; private set; }
        public Conference Conference { get; private set; }
        public string? SubmissionInstruction { get; internal set; }
        public string? SubmissionSettings { get; internal set; }
        public string? ConflictSettings { get; internal set; }
        public string? ReviewSettings { get; internal set; }
        public string? CameraReadySubmissionSettings { get; internal set; }
        public string? SubjectAreaRelevanceCoefficients { get; set; }

        public ICollection<Incumbent> Incumbents { get; set; }
        public ICollection<SubjectArea> SubjectAreas { get; set; }
        public ICollection<Submission> Submissions { get; set; }
        public ICollection<ConflictCase> ConflictCases { get; set; }
        public ICollection<QuestionGroupTrack> QuestionGroups { get; set; }
        public ICollection<ActivityDeadline> ActivityDeadlines { get; set; }

        public Track(
        Guid id,
        bool isDefault,
        string name,
        Guid conferenceId,
        string? submissionInstruction,
        string? submissionSettings,
        string? conflictSettings,
        string? reviewSettings,
        string? cameraReadySubmissionSettings,
        string? subjectAreaRelevanceCoefficients)
            : base(id)
        {
            IsDefault = isDefault;
            SetName(name);
            ConferenceId = conferenceId;
            SetSubmissionInstruction(submissionInstruction);
            SubmissionSettings = submissionSettings;
            ConflictSettings = conflictSettings;
            ReviewSettings = reviewSettings;
            CameraReadySubmissionSettings = cameraReadySubmissionSettings;
            SubjectAreaRelevanceCoefficients = subjectAreaRelevanceCoefficients;

            Incumbents = new Collection<Incumbent>();
            SubjectAreas = new Collection<SubjectArea>();
            Submissions = new Collection<Submission>();
            ConflictCases = new Collection<ConflictCase>();
            QuestionGroups = new Collection<QuestionGroupTrack>();
            ActivityDeadlines = new Collection<ActivityDeadline>();
        }

        //public Track(Guid id, bool isDefault, string name, Guid conferenceId) : base(id)
        //{
        //    IsDefault = isDefault;
        //    SetName(name);
        //    ConferenceId = conferenceId;

        //    SetSubmissionInstruction(null);
        //    SubmissionSettings = null;
        //    ConflictSettings = null;
        //    ReviewSettings = null;
        //    CameraReadySubmissionSettings = null;
        //    SubjectAreaRelevanceCoefficients = null;

        //    Incumbents = new Collection<Incumbent>();
        //    SubjectAreas = new Collection<SubjectArea>();
        //    Submissions = new Collection<Submission>();
        //    ConflictCases = new Collection<ConflictCase>();
        //    QuestionGroups = new Collection<QuestionGroupTrack>();
        //    ActivityDeadlines = new Collection<ActivityDeadline>();
        //}

        public Track SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(string.IsNullOrEmpty(name) ? name : name.Trim(), nameof(name), TrackConsts.MaxNameLength);
            return this;
        }

        public Track SetSubmissionInstruction(string? submissionInstruction)
        {
            if (string.IsNullOrWhiteSpace(submissionInstruction))
            {
                SubmissionInstruction = null;
            }
            else
            {
                SubmissionInstruction = Check.NotNullOrWhiteSpace(submissionInstruction, nameof(submissionInstruction), TrackConsts.MaxSubmissionInstructionLength);
            }
            return this;
        }

        public Track AddSubjectArea(Guid subjectAreaId, string subjectAreaName)
        {
            if(SubjectAreas.Any(x => x.Name.EqualsIgnoreCase(string.IsNullOrEmpty(subjectAreaName) ? subjectAreaName : subjectAreaName.Trim())))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubjectAreaAlreadyExistToTrack);
            }

            SubjectAreas.Add(new SubjectArea(subjectAreaId, subjectAreaName, Id));

            return this;
        }

        public Track UpdateSubjectArea(Guid subjectAreaId, string subjectAreaName)
        {
            var subjectArea = SubjectAreas.SingleOrDefault(x => x.Id == subjectAreaId);
            if(subjectArea == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubjectAreaNotFound);
            }
            else if(SubjectAreas.Any(x => x.Name.EqualsIgnoreCase(
                string.IsNullOrEmpty(subjectAreaName) ? subjectAreaName : subjectAreaName.Trim()) 
            && x.Id != subjectAreaId))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubjectAreaAlreadyExistToTrack);
            }

            subjectArea.SetName(subjectAreaName);

            return this;
        }

        public Track AddSubmission(Guid submissionId, string title, 
            string @abstract, string rootFilePath, 
            string? domainConflicts, Guid? createdIncumbentId, 
            string? answers, Guid statusId)
        {
            if(Submissions.Any(x => x.Title.EqualsIgnoreCase(string.IsNullOrEmpty(title) ? title : title.Trim())
            && x.Abstract.EqualsIgnoreCase(string.IsNullOrEmpty(@abstract) ? @abstract : @abstract.Trim())))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionAlreadyExistToTrack);
            }

            Submissions.Add(new Submission(submissionId, title, @abstract, rootFilePath, Id, domainConflicts, 
                createdIncumbentId, createdIncumbentId, answers, statusId, null, null, null, false, false, null, false, null));

            return this;
        }

        public Track AddSubmission(Submission submission)
        {
            if (Submissions.Any(x => x.Title.EqualsIgnoreCase(string.IsNullOrEmpty(submission.Title) ? submission.Title : submission.Title.Trim())
            && x.Abstract.EqualsIgnoreCase(string.IsNullOrEmpty(submission.Abstract) ? submission.Abstract : submission.Abstract.Trim())))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionAlreadyExistToTrack);
            }

            Submissions.Add(submission);

            return this;
        }
    }
}
