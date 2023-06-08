using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Volo.Abp.Identity;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using System.Linq;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceAccount : FullAuditedAggregateRoot<Guid>
    {
        public Guid ConferenceId { get; private set; }
        public Conference Conference { get; private set; }
        public Guid AccountId { get; private set; }
        public IdentityUser Account { get; private set; }
        public bool HasDomainConflictConfirmed { get; internal set; }

        public ICollection<Incumbent> Incumbents { get; private set; }

        public ConferenceAccount(
            Guid id,
            Guid conferenceId,
            Guid accountId,
            bool hasDomainConflictConfirmed)
            : base(id)
        {
            ConferenceId = conferenceId;
            AccountId = accountId;
            HasDomainConflictConfirmed = hasDomainConflictConfirmed;

            Incumbents = new Collection<Incumbent>();
        }

        public ConferenceAccount AddIncumbent(
            Guid incumbentId,
            Guid conferenceRoleId,
            Guid? trackId,
            bool isPrimaryContact)
        {
            if (trackId == null)
            {
                if (Incumbents.Any(x => x.ConferenceAccountId == Id
                && x.ConferenceRoleId == conferenceRoleId))
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.IncumbentAlreadyAssigned);
                }
            }
            else if (Incumbents.Any(x => x.ConferenceAccountId == Id
                && x.ConferenceRoleId == conferenceRoleId && x.TrackId == trackId))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.IncumbentAlreadyAssigned);
            }

            Incumbents.Add(new Incumbent(incumbentId, Id, conferenceRoleId, trackId, isPrimaryContact));

            return this;
        }

        public ConferenceAccount AddIncumbent(Incumbent incumbent)
        {
            if (incumbent.TrackId == null)
            {
                if (Incumbents.Any(x => x.ConferenceAccountId == Id
                && x.ConferenceRoleId == incumbent.ConferenceRoleId))
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.IncumbentAlreadyAssigned);
                }
            }
            else if (Incumbents.Any(x => x.ConferenceAccountId == Id
                && x.ConferenceRoleId == incumbent.ConferenceRoleId && x.TrackId == incumbent.TrackId))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.IncumbentAlreadyAssigned);
            }

            Incumbents.Add(incumbent);

            return this;
        }

        public ConferenceAccount UpdateIncumbent(
            Guid incumbentId,
            bool isPrimaryContact)
        {
            var found = Incumbents.SingleOrDefault(x => x.Id == incumbentId);
            if (found == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.IncumbentNotFound);
            }

            found.IsPrimaryContact = isPrimaryContact;

            return this;
        }

        public ConferenceAccount DeleteIncumbent(Guid incumbentId)
        {
            var incumbent = Incumbents.SingleOrDefault(x => x.Id == incumbentId);
            if (incumbent == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.IncumbentNotFound);
            }

            Incumbents.Remove(incumbent);

            return this;
        }
    }
}
