using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ITimeAppService : IApplicationService
    {
        DateTime GetNow();

        Task<DateTime> SetNow(DateTime now);

        DateTime Reset();
    }
}
