using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Conference")]
    [Route("api/sras/conferences")]
    public class ConferenceController : AbpController
    {
        private readonly IConferenceAppService _conferenceService;

        public ConferenceController(IConferenceAppService conferenceService)
        {
            _conferenceService = conferenceService;
        }

        [HttpPost]
        public async Task<ConferenceWithDetails> CreateAsync(ConferenceWithDetailsInput input)
        {
            return await _conferenceService.CreateAsync(input);
        }

        [HttpPost("{id}")]
        public async Task<ConferenceWithDetails> UpdateAsync(Guid id, ConferenceWithDetailsInput input)
        {
            return await _conferenceService.UpdateAsync(id, input);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            return Ok(new { message = await _conferenceService.DeleteAsync(id) ? "Delete Successfully" : "Delete Failed" });
        }

        [HttpGet]
        public Task<PagedResultDto<ConferenceWithBriefInfo>> GetListAsync(ConferenceListFilterDto filter)
        {
            return _conferenceService.GetListAsync(filter);
        }

        [HttpGet("{id}")]
        public async Task<ConferenceWithDetails> GetAsync(Guid id)
        {
            return await _conferenceService.GetAsync(id);
        }

        [HttpGet("{id}/users")]
        public async Task<List<ConferenceParticipationBriefInfo>> GetListConferenceUsersAsync(Guid id, ConferenceParticipationFilterDto input)
        {
            return await _conferenceService.GetConferenceUserListAsync(id, input);
        }

        [HttpGet("numOfSubmission/{conferenceId}")]
        public async Task<object> GetNumberOfSubmission(Guid conferenceId, Guid? trackId)
        {
            var result = await _conferenceService.GetNumberOfSubmission(conferenceId, trackId);
            return result;
        }
    }
}
