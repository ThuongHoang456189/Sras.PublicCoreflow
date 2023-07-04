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
        public async Task<string?> UpdateSubmissionSettingsAsync(Guid id, SettingsDto? submissionSettings, string? submissionInstruction)
        {
            return await _trackAppService.UpdateSubmissionSettingsAsync(id, submissionSettings?.Settings, submissionInstruction);
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
        public async Task<string?> UpdateConflictSettingsAsync(Guid id, SettingsDto? conflictSettings)
        {
            return await _trackAppService.UpdateConflictSettingsAsync(id, conflictSettings?.Settings);
        }

        [HttpGet("{id}/conflict-settings")]
        public async Task<string?> GetConflictSettingsAsync(Guid id)
        {
            return await _trackAppService.GetConflictSettingsAsync(id);
        }

        [HttpPost("{id}/review-settings")]
        public async Task<string?> UpdateReviewSettingsAsync(Guid id, SettingsDto? reviewSettings)
        {
            return await _trackAppService.UpdateReviewSettingsAsync(id, reviewSettings?.Settings);
        }

        [HttpGet("{id}/review-settings")]
        public async Task<string?> GetReviewSettingsAsync(Guid id)
        {
            return await _trackAppService.GetReviewSettingsAsync(id);
        }

        [HttpPost("{id}/revision-settings")]
        public async Task<string?> UpdateRevisionSettingsAsync(Guid id, SettingsDto? revisionSettings)
        {
            return await _trackAppService.UpdateRevisionSettingsAsync(id, revisionSettings?.Settings);
        }

        [HttpDelete("{id}/revision-settings")]
        public async Task<IActionResult> DeleteRevisionSettingsAsync(Guid id)
        {
            await _trackAppService.DeleteRevisionSettingsAsync(id);

            return Ok("Deleted Successfully!");
        }

        [HttpGet("{id}/revision-settings")]
        public async Task<string?> GetRevisionSettingsAsync(Guid id)
        {
            return await _trackAppService.GetRevisionSettingsAsync(id);
        }

        [HttpPost("{id}/decision-checklist")]
        public async Task<string?> UpdateDecisionChecklistAsync(Guid id, ChecklistDto? decisionChecklist)
        {
            return await _trackAppService.UpdateDecisionChecklistAsync(id, decisionChecklist?.Checklist);
        }

        [HttpGet("{id}/decision-checklist")]
        public async Task<string?> GetDecisionChecklistAsync(Guid id)
        {
            return await _trackAppService.GetDecisionChecklistAsync(id);
        }

        [HttpPost("{id}/camera-ready-submission-settings")]
        public async Task<string?> UpdateCameraReadySubmissionSettingsAsync(Guid id, SettingsDto? cameraReadySubmissionSettings)
        {
            return await _trackAppService.UpdateCameraReadySubmissionSettingsAsync(id, cameraReadySubmissionSettings?.Settings);
        }

        [HttpGet("{id}/camera-ready-submission-settings")]
        public async Task<string?> GetCameraReadySubmissionSettingsAsync(Guid id)
        {
            return await _trackAppService.GetCameraReadySubmissionSettingsAsync(id);
        }

        [HttpPost("{id}/presentation-settings")]
        public async Task<string?> UpdatePresentationSettingsAsync(Guid id, SettingsDto? presentationSettings)
        {
            return await _trackAppService.UpdatePresentationSettingsAsync(id, presentationSettings?.Settings);
        }

        [HttpGet("{id}/presentation-settings")]
        public async Task<string?> GetPresentationSettingsAsync(Guid id)
        {
            return await _trackAppService.GetPresentationSettingsAsync(id);
        }

        [HttpPost("{id}/track-plan-with-number-of-revisions")]
        public async Task<List<TrackPlanRecordInput>> InitializeTrackPlan(Guid id, int numberOfRevisions)
        {
            return await _trackAppService.InitializeTrackPlan(id, numberOfRevisions);
        }

        [HttpGet("{id}/track-plan")]
        public async Task<List<TrackPlanRecordInput>> GetInitialTrackPlan(Guid id)
        {
            return await _trackAppService.GetInitialTrackPlan(id);
        }

        [HttpPost("{id}/track-plan")]
        public async Task<List<TrackPlanRecordInput>> SaveTrackPlanAsync(Guid id, List<TrackPlanRecordInput> trackPlanRecords)
        {
            return await _trackAppService.SaveTrackPlanAsync(id, trackPlanRecords);
        }

        [HttpGet("{id}/activity-timeline")]
        public async Task<List<TrackPlanRecordInput>> GetTrackActivityTimeline(Guid id)
        {
            return await _trackAppService.GetTrackActivityTimeline(id);
        }

        [HttpPost("{id}/activity-timeline/extension")]
        public async Task<List<TrackPlanRecordInput>> ExtendActivityDeadline(Guid id, TrackPlanRecordInput activityDeadline)
        {
            return await _trackAppService.ExtendActivityDeadline(id, activityDeadline);
        }

        [HttpPost("{id}/activity-timeline/completion")]
        public async Task<TrackPlanRecordInput> CompleteActivityDeadlineAsync(Guid id, Guid activityDeadlineId)
        {
            return await _trackAppService.CompleteActivityDeadlineAsync(id, activityDeadlineId);
        }

        [HttpGet("{id}/guidelines")]
        public async Task<GuidelineGroupDto> GetGuidelines(Guid id, bool isForChair)
        {
            return await _trackAppService.GetGuidelines(id, isForChair);
        }

        [HttpPost("{id}/submission-questions")]
        public async Task<IActionResult> CreateOrUpdateQuestionListAsync(Guid id, QuestionListInput input)
        {
            return Ok(await _trackAppService.CreateOrUpdateQuestionListAsync(id, input));
        }

        [HttpGet("{id}/submission-questions")]
        public async Task<List<QuestionDto>> GetSubmissionQuestionListAsync(Guid id)
        {
            return await _trackAppService.GetSubmissionQuestionListAsync(id);
        }

        [HttpGet("{id}/decision-checklist-questions")]
        public async Task<List<QuestionDto>> GetDecisionChecklistQuestionsAsync(Guid id)
        {
            return await _trackAppService.GetDecisionChecklistQuestionsAsync(id);
        }

        [HttpGet("{id}/camera-ready-checklist-questions")]
        public async Task<List<QuestionDto>> GetCameraReadyChecklistQuestionsAsync(Guid id)
        {
            return await _trackAppService.GetCameraReadyChecklistQuestionsAsync(id);
        }
    }
}
