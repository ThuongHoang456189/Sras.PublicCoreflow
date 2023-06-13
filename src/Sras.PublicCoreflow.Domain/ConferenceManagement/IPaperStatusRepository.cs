using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IPaperStatusRepository
    {
        Task<IEnumerable<object>> GetAllPaperStatus(Guid? conferenceId);
        Task<IEnumerable<PaperStatus>> GetPaperStatusesAllField(Guid conferenceId);
        Task<object> CreatePaperStatus(PaperStatusCreateRequest createRequest);
    }
}
