using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ISrasBackgroundAppService : IApplicationService
    {
        Task UpdateActivityTimelineAsync();
    }
}
