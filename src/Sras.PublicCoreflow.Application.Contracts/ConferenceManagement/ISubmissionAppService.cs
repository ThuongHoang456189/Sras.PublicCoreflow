using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;
using System.Collections.Generic;
using Volo.Abp.Content;
using Sras.PublicCoreflow.Dto;
using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ISubmissionAppService : IApplicationService
    {
        Task<Guid> CreateAsync(SubmissionInput input);
        ResponseDto CreateSubmissionFiles(Guid submissionId, List<RemoteStreamContent> files);
        Task<object> GetNumberOfSubmission(Guid trackId);
        Task<object> GetNumberOfSubmissionAndEmail(SubmissionWithEmailRequest request);
        Task<IEnumerable<object>> GetSubmissionsAsync();
        Task<ResponseDto> UpdateSubmissionConflict(Guid submissionId, List<ConflictInput> conflicts);
        Task<PagedResultDto<ReviewerWithConflictDetails>> GetListReviewerWithConflictDetails(Guid id);
        Task<SubmissionReviewerAssignmentSuggestionDto> GeSubmissionReviewerAssignmentSuggestionAsync(Guid submissionId);
        Task<CreationResponseDto> CreateRevisionAsync(Guid submissionId, List<RemoteStreamContent> files);
        Task<ResponseDto> AssignReviewerAsync(Guid submissionId, Guid reviewerId, bool isAssigned);
        Task<ResponseDto> DecideOnPaper(Guid submissionId, Guid paperStatusId);
        Task<PagedResultDto<SubmissionAggregation>> GetListSubmissionAggregation(SubmissionAggregationListFilterDto filter);
    }
}
