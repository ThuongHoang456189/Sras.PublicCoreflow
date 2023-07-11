using System;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ITimeAppService : IApplicationService
    {
        DateTime GetNow();

        DateTime SetNow(DateTime now);

        DateTime Reset();
    }
}
