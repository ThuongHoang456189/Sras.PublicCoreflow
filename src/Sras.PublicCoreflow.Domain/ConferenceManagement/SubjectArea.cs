using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubjectArea : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public Guid TrackId { get; private set; }
        public Track Track { get; private set; }

        public ICollection<SubmissionSubjectArea> Submissions { get; private set; }
        public ICollection<ConferenceReviewerSubjectArea> ConferenceReviewers { get; private set; }

        public SubjectArea(Guid id, string name, Guid trackId) : base(id) 
        {
            SetName(name);
            TrackId = trackId;

            Submissions = new Collection<SubmissionSubjectArea>();
            ConferenceReviewers = new Collection<ConferenceReviewerSubjectArea>();
        }

        public SubjectArea SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(string.IsNullOrEmpty(name) ? name : name.Trim(), nameof(name), SubjectAreaConsts.MaxNameLength);
            return this;
        }
    }
}
