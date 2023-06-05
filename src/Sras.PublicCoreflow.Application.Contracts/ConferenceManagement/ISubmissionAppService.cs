using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ISubmissionAppService : IApplicationService
    {
        void Create(SubmissionInput input);
    }
}
