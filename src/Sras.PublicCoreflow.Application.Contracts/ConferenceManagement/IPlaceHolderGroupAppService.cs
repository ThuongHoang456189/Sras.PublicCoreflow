using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IPlaceHolderGroupAppService : IApplicationService
    {
        Task<IEnumerable<object>> GetAllSupportedPlaceHolderAsync();
    }
}
