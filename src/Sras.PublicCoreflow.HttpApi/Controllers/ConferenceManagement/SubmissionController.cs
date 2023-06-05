using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Submission")]
    [Route("api/sras/submissions")]
    public class SubmissionController : AbpController
    {
        private readonly ISubmissionAppService _submissionAppService;

        public SubmissionController(ISubmissionAppService submissionAppService)
        {
            _submissionAppService = submissionAppService;
        }

        // Need to fix response to show submission summary
        [HttpPost]
        public async Task<IActionResult> CreateAsync ([FromForm] SubmissionInput input)
        {
            _submissionAppService.Create(input);
            return Ok(new { message = "Hello" });
        }
    }
}
