using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

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

        [HttpPost]
        public async Task<Guid> CreateAsync (SubmissionInput input)
        {
            return await _submissionAppService.CreateAsync(input);
        }

        [HttpPost("{id}/submission-files")]
        public IActionResult CreateSubmissionFiles (Guid id, [FromForm] List<RemoteStreamContent> files)
        {
            return Ok(_submissionAppService.CreateSubmissionFiles(id, files));
        }
    }
}
