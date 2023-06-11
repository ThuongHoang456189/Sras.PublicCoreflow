using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IReviewerAppService : IApplicationService
    {
        Task<ResponseDto> UpdateReviewerQuota(ReviewerQuotaInput input);

        Task<ResponseDto> UpdateReviewerSubjectArea(ReviewerSubjectAreaInput input);

        Task<ResponseDto> UpdateReviewerConflict(ReviewerConflictInput input);

        Task<List<ConflictWithDetails>> GetListReviewerConflictAsync(ReviewerConflictLookUpInput input);
    }
}
