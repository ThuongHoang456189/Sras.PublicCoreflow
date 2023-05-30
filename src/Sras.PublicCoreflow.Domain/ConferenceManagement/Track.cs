using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Track : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public Guid ConferenceId { get; private set; }
        public Conference Conference { get; private set; }
        public string? SubmissionInstruction { get; internal set; }
        public string? SubmissionSettings { get; internal set; }
        public string? ConflictSettings { get; internal set; }
        public string? ReviewSettings { get; internal set; }
        public string? CameraReadySubmissionSettings { get; internal set; }
        public string? SubjectAreaRelevanceCoefficients { get; internal set; }

        public ICollection<Incumbent> Incumbents { get; private set; }

        public Track(
        Guid id,
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
            SetName(name);
            ConferenceId = conferenceId;
            SetSubmissionInstruction(submissionInstruction);
            SubmissionSettings = submissionSettings;
            ConflictSettings = conflictSettings;
            ReviewSettings = reviewSettings;
            CameraReadySubmissionSettings = cameraReadySubmissionSettings;
            SubjectAreaRelevanceCoefficients = subjectAreaRelevanceCoefficients;

            Incumbents = new Collection<Incumbent>();
        }

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
    }
}
