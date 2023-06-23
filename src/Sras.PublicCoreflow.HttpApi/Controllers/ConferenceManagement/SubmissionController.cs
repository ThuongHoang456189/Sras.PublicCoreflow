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

        [HttpGet("{id}/submission-files")]
        public async Task<IActionResult> GetSubmissionFilesAsync(Guid id)
        {
            return File(await _submissionAppService.DownloadSubmissionFiles(id), "application/zip", id.ToString() + ".zip");
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
        public async Task<SubmissionReviewerConflictDto> GetListReviewerWithConflictDetails(Guid id)
        {
            return await _submissionAppService.GetListReviewerWithConflictDetails(id);
        }

        [HttpGet("{id}/reviewer-assignment")]
        public async Task<SubmissionReviewerAssignmentSuggestionDto> GeSubmissionReviewerAssignmentSuggestionAsync(Guid id)
        {
            return await _submissionAppService.GeSubmissionReviewerAssignmentSuggestionAsync(id);
        }

        [HttpPost("{id}/reviewer-assignment")]
        public async Task<IActionResult> UpdateSubmissionReviewAssignment(Guid id, Guid reviewerId, bool isAssigned)
        {
            return Ok(await _submissionAppService.AssignReviewerAsync(id, reviewerId, isAssigned));
        }

        [HttpPost("{id}/revisions")]
        public async Task<IActionResult> CreateRevision(Guid id, [FromForm] List<RemoteStreamContent> files)
        {
            return Ok(await _submissionAppService.CreateRevisionAsync(id, files));
        }

        [HttpPost("{id}/camera-ready")]
        public async Task<IActionResult> CreateCameraReady(Guid id, [FromForm] List<RemoteStreamContent> files)
        {
            return Ok(await _submissionAppService.CreateCameraReadyAsync(id, files));
        }

        [HttpPost("{id}/decision")]
        public async Task<IActionResult> UpdateDecision(Guid id, Guid paperStatusId)
        {
            return Ok(await _submissionAppService.DecideOnPaper(id, paperStatusId));
        }

        [HttpGet("aggregation")]
        public async Task<PagedResultDto<SubmissionAggregation>> GetListSubmissionAggregation(SubmissionAggregationListFilterDto filter)
        {
            return await _submissionAppService.GetListSubmissionAggregation(filter);
        }

        [HttpPost("{id}/camera-ready-request")]
        public async Task<IActionResult> RequestCameraReady(Guid id, bool isCameraReadyRequested)
        {
            return Ok(await _submissionAppService.RequestCameraReady(id, isCameraReadyRequested));
        }

        [HttpGet("sp-aggregation")]
        public async Task<PagedResultDto<SubmissionAggregationDto>> GetListSubmissionAggregationSP(string? inclusionText, Guid conferenceId, Guid? trackId, Guid? statusId, int skipCount, int maxResultCount)
        {
            return await _submissionAppService.GetListSubmissionAggregationSP(inclusionText, conferenceId, trackId, statusId, skipCount, maxResultCount);
        }
    }
}
