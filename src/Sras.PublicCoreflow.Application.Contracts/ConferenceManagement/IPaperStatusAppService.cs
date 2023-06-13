using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IPaperStatusAppService
    {
        Task<IEnumerable<object>> GetAllPaperStatusAsync(Guid? conferenceId);
        Task<object> CreatePaperStatusAsync(PaperStatusCreateRequest createRequest);
    }
}
