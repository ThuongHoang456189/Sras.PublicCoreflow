using System.Threading.Tasks;
using System;
using Volo.Abp.Identity;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IUserAppService
    {
        Task<List<IdentityRoleDto>> GetRolesAsync(Guid id);
    }
}