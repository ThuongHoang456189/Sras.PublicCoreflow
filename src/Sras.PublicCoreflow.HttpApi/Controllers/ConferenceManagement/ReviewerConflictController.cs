using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("ReviewerConflict")]
    [Route("api/sras/reviewer-conflicts")]
    public class ReviewerConflictController : AbpController
    {
        private readonly IReviewerAppService _reviewerAppService;

        public ReviewerConflictController(IReviewerAppService reviewerAppService)
        {
            _reviewerAppService = reviewerAppService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(ReviewerConflictInput input)
        {
            return Ok(await _reviewerAppService.UpdateReviewerConflict(input));
        }

        [HttpGet]
        public async Task<ReviewerSubmissionConflictDto> GetListReviewerConflictAsync(ReviewerConflictLookUpInput input)
        {
            return await _reviewerAppService.GetListReviewerConflictAsync(input);
        }
    }
}
