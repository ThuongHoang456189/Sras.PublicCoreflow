using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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

        [HttpPost("request-camera-ready/{submissionId}/{status}")]
        public async Task<IActionResult> RequestForCameraReady(Guid submissionId, bool status)
        {/* status == true => Yes;
            status == false => No; */
            try
            {
                if (submissionId == null)
                {
                    throw new Exception("SubmissionId is Null");
                }
                else
                {
                    var result = await _submissionAppService.UpdateStatusRequestForCameraReady(submissionId, status);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("request-all-camera-ready/{conferenceId}/{status}")]
        public async Task<IActionResult> RequestAllForCameraReady(Guid conferenceId, bool status)
        {/* status == true => Yes;
            status == false => No; */
            try
            {
                if (conferenceId == null)
                {
                    throw new Exception("ConferenceId is Null");
                }
                else
                {

                    var result = await _submissionAppService.UpdateStatusRequestForAllCameraReady(conferenceId, status);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
