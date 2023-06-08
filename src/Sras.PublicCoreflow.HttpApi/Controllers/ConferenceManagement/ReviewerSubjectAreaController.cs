using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("ReviewerSubjectArea")]
    [Route("api/sras/reviewer-subject-areas")]
    public class ReviewerSubjectAreaController : AbpController
    {
        private readonly IReviewerAppService _reviewerAppService;

        public ReviewerSubjectAreaController(IReviewerAppService reviewerAppService)
        {
            _reviewerAppService = reviewerAppService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(ReviewerSubjectAreaInput input)
        {
            return Ok(await _reviewerAppService.UpdateReviewerSubjectArea(input));
        }
    }
}
