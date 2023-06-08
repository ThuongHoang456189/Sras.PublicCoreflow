using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("ReviewerQuota")]
    [Route("api/sras/reviewer-quotas")]
    public class ReviewerQuotaController : AbpController
    {
        private readonly IReviewerAppService _reviewerAppService;

        public ReviewerQuotaController(IReviewerAppService reviewerAppService)
        {
            _reviewerAppService = reviewerAppService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(ReviewerQuotaInput input)
        {
            return Ok(await _reviewerAppService.UpdateReviewerQuota(input));
        }
    }
}
