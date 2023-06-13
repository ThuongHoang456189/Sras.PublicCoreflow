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
        Task<object> GetNumberOfSubmissionAndEmail(SubmissionWithEmailRequest request);
        Task<IEnumerable<object>> GetSubmissionsAsync();
        Task<ResponseDto> UpdateSubmissionConflict(Guid submissionId, List<ConflictInput> conflicts);
        Task<PagedResultDto<ReviewerWithConflictDetails>> GetListReviewerWithConflictDetails(Guid id);

        Task<object> UpdateStatusRequestForCameraReady(Guid submissionId, bool status);
        Task<object> UpdateStatusRequestForAllCameraReady(Guid conferenceId, bool status);
    }
}
