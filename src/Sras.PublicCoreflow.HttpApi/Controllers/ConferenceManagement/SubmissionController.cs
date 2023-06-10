using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
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

        [HttpGet("numOfSubmission/{trackId}")]
        public async Task<ActionResult<object>> GetNumberOfSubmissionByTrackId(Guid trackId)
        {
            try
            {
                if (trackId == null)
                {
                    throw new Exception("Invalid TrackId");
                } else
                {
                    var result = await _submissionAppService.GetNumberOfSubmission(trackId);
                    return Ok(result);
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("get-number-submissions-and-email-by-status")]
        public async Task<ActionResult<object>> GetNumberOfSubmissionAndEmail([FromBody] SubmissionWithEmailRequest request)
        {
            try
            {
                if (request == null)
                {
                    throw new Exception("Request is Null");
                }
                var result = _submissionAppService.GetNumberOfSubmissionAndEmail(request);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSubmissionsAsync()
        {
            var result = await _submissionAppService.GetSubmissionsAsync();
            return Ok(result);
        }

        [HttpPost("{id}/conflicts")]
        public async Task<IActionResult> UpdateSubmissionConflict(Guid id, List<ConflictInput> conflicts)
        {
            return Ok(await _submissionAppService.UpdateSubmissionConflict(id, conflicts));
        }

        [HttpGet("{id}/conflicts")]
        public async Task<PagedResultDto<ReviewerWithConflictDetails>> GetListReviewerWithConflictDetails(Guid id)
        {
            return await _submissionAppService.GetListReviewerWithConflictDetails(id);
        }
    }
}
