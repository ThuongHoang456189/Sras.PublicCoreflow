using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ISubjectAreaAppService : IApplicationService
    {
        Task CreateAsync(SubjectAreaInput input);

        Task UpdateAsync(Guid subjectAreaId, SubjectAreaInput input);

        Task DeleteAsync(Guid subjectAreaId);

        Task<List<SubjectAreaBriefInfo>> GetListAsync(Guid trackId);
    }
}
