using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IAccountRepository : IRepository<IdentityUser, Guid>
    {
        bool ConfirmEmail(Guid id);
        IEnumerable<RegisterAccountRequest> GetAllAccount();
        bool UpdateAccount(RegisterAccountRequest registerAccount);
    }
}
