using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class IncumbentOperation
    {
        public Guid ConferenceAccountId { get; set; }
        public Guid AccountId { get; set; }
        public Guid IncumbentId { get; set; }
        public Guid ConferenceRoleId { get; set; }
        public IncumbentManipulationOperators Operation { get; set; } = IncumbentManipulationOperators.None;
        public bool IsPrimaryContact { get; set; }

        public IncumbentOperation(Guid conferenceAccountId, Guid accountId, Guid incumbentId, Guid conferenceRoleId, IncumbentManipulationOperators operation, bool isPrimaryContact)
        {
            ConferenceAccountId = conferenceAccountId;
            AccountId = accountId;
            IncumbentId = incumbentId;
            ConferenceRoleId = conferenceRoleId;
            Operation = operation;
            IsPrimaryContact = isPrimaryContact;
        }
    }
}
