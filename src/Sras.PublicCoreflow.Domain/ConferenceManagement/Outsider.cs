using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Outsider : FullAuditedAggregateRoot<Guid>
    {
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string? MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string? Organization { get; private set; }
        public Guid ParticipantId { get; private set; }
        public Participant Participant { get; private set; }

        public Outsider(Guid id, string email, string firstName, string? middleName, string lastName, string? organization, Guid participantId) : base(id) 
        {
            SetEmail(email);
            SetFirstName(firstName);
            SetMiddleName(middleName);
            SetLastName(lastName);
            SetOrganization(organization);
            ParticipantId = participantId;
        }

        public Outsider(Guid id, string email, string firstName, string? middleName, string lastName, string? organization) : base(id)
        {
            SetEmail(email);
            SetFirstName(firstName);
            SetMiddleName(middleName);
            SetLastName(lastName);
            SetOrganization(organization);
        }

        public Outsider SetEmail (string email)
        {
            Email = Check.NotNullOrWhiteSpace(string.IsNullOrEmpty(email) ? email : email.Trim(), nameof(email), OutsiderConsts.MaxEmailLength);
            return this;
        }

        public Outsider SetFirstName(string firstName)
        {
            FirstName = Check.NotNullOrWhiteSpace(string.IsNullOrEmpty(firstName) ? firstName : firstName.Trim(), nameof(firstName), OutsiderConsts.MaxFirstNameLength);
            return this;
        }

        public Outsider SetMiddleName(string? middleName)
        {
            middleName = string.IsNullOrEmpty(middleName) ? middleName : middleName.Trim();
            MiddleName = string.IsNullOrEmpty(middleName) ? middleName : Check.NotNullOrWhiteSpace(middleName, nameof(middleName), OutsiderConsts.MaxMiddleNameLength);
            return this;
        }

        public Outsider SetLastName(string lastName)
        {
            LastName = Check.NotNullOrWhiteSpace(string.IsNullOrEmpty(lastName) ? lastName : lastName.Trim(), nameof(lastName), OutsiderConsts.MaxLastNameLength);
            return this;
        }

        public Outsider SetOrganization(string? organization)
        {
            organization = string.IsNullOrEmpty(organization) ? organization : organization.Trim();
            Organization = string.IsNullOrEmpty(organization) ? organization : Check.NotNullOrWhiteSpace(organization, nameof(organization), OutsiderConsts.MaxOrganizationLength);
            return this;
        }
    }
}

