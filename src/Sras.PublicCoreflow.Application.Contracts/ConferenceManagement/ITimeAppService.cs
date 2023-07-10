using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ITimeAppService : IApplicationService
    {
        string GetTimeZone();
    }
}
