using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IReviewerAppService : IApplicationService
    {
        Task<ResponseDto> UpdateReviewerQuota(ReviewerQuotaInput input);

        Task<ResponseDto> UpdateReviewerSubjectArea(ReviewerSubjectAreaInput input);
    }
}
