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

        [HttpGet("get-tracks-and-roles-of-chackchair-user/{userId}/{conferenceId}")]
        public async Task<ActionResult<object>> GetTracksAndRoleOfUser(Guid userId, Guid conferenceId, string? roleName)
        {
            try
            {
                var result = _trackAppService.GetTracksAndRoleOfUser(userId, conferenceId, roleName);
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
    }
}
