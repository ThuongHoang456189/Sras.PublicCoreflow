using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IReviewerAppService : IApplicationService
    {
        Task<ResponseDto> UpdateReviewerQuota(ReviewerQuotaInput input);

        Task<ResponseDto> UpdateReviewerSubjectArea(ReviewerSubjectAreaInput input);

        Task<ResponseDto> UpdateReviewerConflict(ReviewerConflictInput input);

        Task<ReviewerSubmissionConflictDto> GetListReviewerConflictAsync(ReviewerConflictLookUpInput input);

        Task<ResponseDto> UploadReview(Guid reviewAssignmentId, List<RemoteStreamContent> files, int? totalScore);

        Task<PagedResultDto<SubmissionWithFacts>> GetListReviewerAggregation(
            Guid accountId, Guid conferenceId,
            string sorting = ReviewerConsts.DefaultSorting,
            int skipCount = 0,
            int maxResultCount = ReviewerConsts.DefaultMaxResultCount);
    }
}
