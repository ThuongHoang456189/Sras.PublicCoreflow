using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IAuthorAppService : IApplicationService
    {
        Task<PagedResultDto<AuthorSubmission>> GetListAuthorAggregation(
            Guid accountId, Guid conferenceId,
            string sorting = AuthorConsts.DefaultSorting,
            int skipCount = 0,
            int maxResultCount = AuthorConsts.DefaultMaxResultCount);
    }
}
