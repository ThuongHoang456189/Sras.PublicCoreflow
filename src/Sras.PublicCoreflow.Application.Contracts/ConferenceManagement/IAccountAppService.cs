using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IAccountAppService : IApplicationService
    {
        Task<AccountWithBriefInfo?> FindAsync(string email);
    }
}
