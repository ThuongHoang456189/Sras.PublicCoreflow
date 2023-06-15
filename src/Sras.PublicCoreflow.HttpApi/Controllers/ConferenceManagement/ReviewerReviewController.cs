using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Content;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("ReviewerReview")]
    [Route("api/sras/reviewer-reviews")]
    public class ReviewerReviewController : AbpController
    {
        private readonly IReviewerAppService _reviewerAppService;

        public ReviewerReviewController(IReviewerAppService reviewerAppService)
        {
            _reviewerAppService = reviewerAppService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(Guid reviewAssignmentId, int? totalScore, [FromForm] List<RemoteStreamContent> files)
        {
            return Ok(await _reviewerAppService.UploadReview(reviewAssignmentId, files, totalScore));
        }
    }
}
