using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AuthorOperation : AuthorInput
    {
        public Guid? AccountId { get; set; }
        public Guid? ConferenceAccountId { get; set; }
        public Guid? AuthorRoleId { get; set; }
        public AuthorManipulationOperators Operation { get; set; } = AuthorManipulationOperators.None;
    }
}
