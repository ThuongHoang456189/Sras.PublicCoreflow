using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;
using System.Collections.Generic;
using Volo.Abp.Content;
using Sras.PublicCoreflow.Dto;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ISubmissionAppService : IApplicationService
    {
        Task<Guid> CreateAsync(SubmissionInput input);

        ResponseDto CreateSubmissionFiles(Guid submissionId, List<RemoteStreamContent> files);
        Task<object> GetNumberOfSubmission(Guid trackId);
        Task<object> GetNumberOfSubmissionAndEmail(SubmissionWithEmailRequest request);
        Task<IEnumerable<object>> GetSubmissionsAsync();
    }
}
