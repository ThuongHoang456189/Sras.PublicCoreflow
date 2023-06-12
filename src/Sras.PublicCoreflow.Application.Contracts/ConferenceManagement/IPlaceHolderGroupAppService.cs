using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IPlaceHolderGroupAppService
    {
        Task<IEnumerable<object>> GetAllSupportedPlaceHolderAsync();
    }
}
