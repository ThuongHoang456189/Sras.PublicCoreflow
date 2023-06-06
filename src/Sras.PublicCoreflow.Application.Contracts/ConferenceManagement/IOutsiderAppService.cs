using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IOutsiderAppService
    {
        Task<OutsiderCreateResponse> CreateOutsider(OutsiderCreateRequest request);
        Task<IEnumerable<object>> GetAllOutsider();
        Task<object> UpdateOutsider(OutsiderUpdateRequest request);
    }
}