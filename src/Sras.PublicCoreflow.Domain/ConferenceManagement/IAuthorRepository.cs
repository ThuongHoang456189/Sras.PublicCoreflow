using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IAuthorRepository : IRepository<Author, Guid>
    {
        Task<int> GetCountAuthorAggregation(
            Guid accountId, Guid conferenceId);

        Task<List<AuthorSubmission>> GetListAuthorAggregation(
            Guid accountId, Guid conferenceId,
            string sorting = AuthorConsts.DefaultSorting,
            int skipCount = 0,
            int maxResultCount = AuthorConsts.DefaultMaxResultCount);
    }
}
