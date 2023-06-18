using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AuthorAppService : PublicCoreflowAppService, IAuthorAppService
    {
        public IAuthorRepository _authorRepository { get; set; }
        public AuthorAppService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<PagedResultDto<AuthorSubmission>> GetListAuthorAggregation(Guid accountId, Guid conferenceId, string sorting = AuthorConsts.DefaultSorting, int skipCount = 0, int maxResultCount = AuthorConsts.DefaultMaxResultCount)
        {
            var count = await _authorRepository.GetCountAuthorAggregation(accountId, conferenceId);

            var items = await _authorRepository.GetListAuthorAggregation(accountId, conferenceId, sorting, skipCount, maxResultCount);

            return new PagedResultDto<AuthorSubmission>(count, items);
        }

        //public async Task TestSPAsync()
        //{
        //    await _authorRepository.TestSPAsync();
        //}
    }
}
