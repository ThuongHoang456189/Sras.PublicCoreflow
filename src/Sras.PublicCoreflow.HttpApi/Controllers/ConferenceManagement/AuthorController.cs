using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System.Threading.Tasks;
using System;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Author")]
    [Route("api/sras/author-management")]
    public class AuthorController : AbpController
    {
        private readonly IAuthorAppService _authorAppService;

        public AuthorController(IAuthorAppService authorAppService)
        {
            _authorAppService = authorAppService;
        }

        [HttpGet("aggregation")]
        public async Task<PagedResultDto<AuthorSubmission>> GetListAuthorAggregation(
            Guid accountId, Guid conferenceId,
            string? sorting,
            int? skipCount,
            int? maxResultCount)
        {
            return await _authorAppService.GetListAuthorAggregation(accountId, conferenceId, 
                sorting ?? AuthorConsts.DefaultSorting, skipCount ?? 0, maxResultCount ?? AuthorConsts.DefaultMaxResultCount);
        }
    }
}