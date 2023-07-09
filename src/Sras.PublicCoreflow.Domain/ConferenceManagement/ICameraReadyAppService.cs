using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ICameraReadyAppService
    {
        Task<IEnumerable<FileDTO>> downloadOneCameraReadyFile(Guid camId);
    }
}
