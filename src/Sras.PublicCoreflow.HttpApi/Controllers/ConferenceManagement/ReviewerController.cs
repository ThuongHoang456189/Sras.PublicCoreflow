using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Reviewer")]
    [Route("api/sras/reviewer-management")]
    public class ReviewerController : AbpController
    {
        private readonly IReviewerAppService _reviewerAppService;

        public ReviewerController(IReviewerAppService reviewerAppService)
        {
            _reviewerAppService = reviewerAppService;
        }

        [HttpGet("aggregation")]
        public async Task<PagedResultDto<SubmissionWithFacts>> GetListReviewerAggregation(
            Guid accountId, Guid conferenceId,
            string? sorting,
            int? skipCount,
            int? maxResultCount)
        {
            return await _reviewerAppService.GetListReviewerAggregation(accountId, conferenceId, 
                sorting ?? ReviewerConsts.DefaultSorting, skipCount ?? 0, maxResultCount ?? ReviewerConsts.DefaultMaxResultCount);
        }
    }
}
