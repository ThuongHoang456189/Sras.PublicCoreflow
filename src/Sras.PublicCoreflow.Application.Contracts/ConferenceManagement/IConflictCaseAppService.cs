using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IConflictCaseAppService : IApplicationService
    {
        Task<IEnumerable<object>> GetAllConflictCasesAsync([Optional] Guid trackId);
    }
}
