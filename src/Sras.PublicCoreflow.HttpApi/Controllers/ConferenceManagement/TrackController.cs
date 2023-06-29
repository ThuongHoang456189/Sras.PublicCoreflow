using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Tracks")]
    [Route("api/sras/tracks")]
    public class TrackController : AbpController
    {

        private readonly ITrackAppService _trackAppService;

        public TrackController(ITrackAppService trackAppService)
        {
            _trackAppService = trackAppService;
        }

        [HttpGet("{conferenceId}")]
        public async Task<object> GetAllTracks(Guid conferenceId)
        {
            return await _trackAppService.GetAllTrackByConferenceId(conferenceId);
        }

        [HttpPost("{conferenceId}/{trackName}")]
        public async Task<ActionResult<object>> CreateAsync(Guid conferenceId, string trackName)
        {
            try
            {
                var result = await _trackAppService.CreateTrackAsync(conferenceId, trackName);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-tracks-and-roles-of-track-chair-user/{userId}/{conferenceId}")]
        public async Task<ActionResult<object>> GetTracksAndRoleOfUser(Guid userId, Guid conferenceId)
        {
            try
            {
                var result = await _trackAppService.GetTracksAndRoleOfUser(userId, conferenceId);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("{id}/subject-area-relevance")]
        public async Task<object?> UpdateTrackSubjectAreaRelevanceCoefficientsAsync(Guid id, SubjectAreaRelevanceCoefficients input)
        {
            return await _trackAppService.UpdateTrackSubjectAreaRelevanceCoefficientsAsync(id, input);
        }

        [HttpGet("{id}/subject-area-relevance")]
        public async Task<object?> GetTrackSubjectAreaRelevanceCoefficientsAsync(Guid id)
        {
            return await _trackAppService.GetTrackSubjectAreaRelevanceCoefficientsAsync(id);
        }

        [HttpPost("{id}/submission-settings")]
        public async Task<string?> UpdateSubmissionSettingsAsync(Guid id, string? submissionSettings, string? submissionInstruction)
        {
            return await _trackAppService.UpdateSubmissionSettingsAsync(id, submissionSettings, submissionInstruction);
        }

        [HttpGet("{id}/submission-settings")]
        public async Task<string?> GetSubmissionSettingsAsync(Guid id)
        {
            return await _trackAppService.GetSubmissionSettingsAsync(id);
        }

        [HttpGet("{id}/submission-instruction")]
        public async Task<string?> GetSubmissionInstructionAsync(Guid id)
        {
            return await _trackAppService.GetSubmissionInstructionAsync(id);
        }

        [HttpPost("{id}/conflict-settings")]
        public async Task<string?> UpdateConflictSettingsAsync(Guid id, string? conflictSettings)
        {
            return await _trackAppService.UpdateConflictSettingsAsync(id, conflictSettings);
        }

        [HttpGet("{id}/conflict-settings")]
        public async Task<string?> GetConflictSettingsAsync(Guid id)
        {
            return await _trackAppService.GetConflictSettingsAsync(id);
        }

        [HttpPost("{id}/review-settings")]
        public async Task<string?> UpdateReviewSettingsAsync(Guid id, string? reviewSettings)
        {
            return await _trackAppService.UpdateReviewSettingsAsync(id, reviewSettings);
        }

        [HttpGet("{id}/review-settings")]
        public async Task<string?> GetReviewSettingsAsync(Guid id)
        {
            return await _trackAppService.GetReviewSettingsAsync(id);
        }

        [HttpPost("{id}/revision-settings")]
        public async Task<string?> UpdateRevisionSettingsAsync(Guid id, string? revisionSettings)
        {
            return await _trackAppService.UpdateRevisionSettingsAsync(id, revisionSettings);
        }

        [HttpGet("{id}/revision-settings")]
        public async Task<string?> GetRevisionSettingsAsync(Guid id)
        {
            return await _trackAppService.GetRevisionSettingsAsync(id);
        }

        [HttpPost("{id}/decision-checklist")]
        public async Task<string?> UpdateDecisionChecklistAsync(Guid id, string? decisionChecklist)
        {
            return await _trackAppService.UpdateDecisionChecklistAsync(id, decisionChecklist);
        }

        [HttpGet("{id}/decision-checklist")]
        public async Task<string?> GetDecisionChecklistAsync(Guid id)
        {
            return await _trackAppService.GetDecisionChecklistAsync(id);
        }

        [HttpPost("{id}/camera-ready-submission-settings")]
        public async Task<string?> UpdateCameraReadySubmissionSettingsAsync(Guid id, string? cameraReadySubmissionSettings)
        {
            return await _trackAppService.UpdateCameraReadySubmissionSettingsAsync(id, cameraReadySubmissionSettings);
        }

        [HttpGet("{id}/camera-ready-submission-settings")]
        public async Task<string?> GetCameraReadySubmissionSettingsAsync(Guid id)
        {
            return await _trackAppService.GetCameraReadySubmissionSettingsAsync(id);
        }

        [HttpPost("{id}/presentation-settings")]
        public async Task<string?> UpdatePresentationSettingsAsync(Guid id, string? presentationSettings)
        {
            return await _trackAppService.UpdatePresentationSettingsAsync(id, presentationSettings);
        }

        [HttpGet("{id}/presentation-settings")]
        public async Task<string?> GetPresentationSettingsAsync(Guid id)
        {
            return await _trackAppService.GetPresentationSettingsAsync(id);
        }
    }
}
