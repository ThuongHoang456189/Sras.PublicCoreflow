using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;
using System.Collections.Generic;
using Volo.Abp.Content;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ISubmissionAppService : IApplicationService
    {
        Task<Guid> CreateAsync(SubmissionInput input);

        void CreateSubmissionFiles(Guid submissionId, List<RemoteStreamContent> files);
        Task<object> GetNumberOfSubmission(Guid trackId);
    }
}
