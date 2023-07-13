﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly IRegistrationAppService _registrationAppService;

        public ConferenceController(
            IConferenceAppService conferenceService,
            IRegistrationAppService registrationAppService)
        {
            _conferenceService = conferenceService;
            _registrationAppService = registrationAppService;
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
        //public async Task<PagedResultDto<ConferenceParticipationBriefInfo>> GetListConferenceUsersAsync(Guid id, ConferenceParticipationFilterDto input)
        //{
        //    return await _conferenceService.GetConferenceUserListAsync(id, input);
        //}
        public async Task<PagedResultDto<ConferenceUserDto>?> GetListConferenceUserAsync(Guid id, ConferenceUserInput input)
        {
            return await _conferenceService.GetListConferenceUserAsync(id, input);
        }

        [HttpGet("numOfSubmission/{conferenceId}")]
        public async Task<object> GetNumberOfSubmission(Guid conferenceId, Guid? trackId)
        {
            var result = await _conferenceService.GetNumberOfSubmission(conferenceId, trackId);
            return result;
        }

        [HttpGet("{id}/registration-settings")]
        public async Task<PriceTable?> GetPriceTable(Guid id)
        {
            return await _conferenceService.GetPriceTable(id);
        }

        [HttpGet("{id}/registrable-papers")]
        public async Task<RegistrablePaperTable> GetRegistrablePaperTable(Guid id, Guid accountId)
        {
            return await _registrationAppService.GetRegistrablePaperTable(id, accountId);
        }

        [HttpGet("{conferenceId}/detail")]
        public async Task<ActionResult<object>> GetConferenceDetail(Guid conferenceId)
        {
            try
            {
                var result = await _conferenceService.GetConferenceDetail(conferenceId);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ConferenceAccount/{accId}/{conferenceId}")]
        public async Task<object> GetConferenceAccountByAccIdConfId(Guid accId, Guid conferenceId)
        {
            return await _conferenceService.GetConferenceAccountByAccIdConfId(accId, conferenceId);
        }

        [HttpGet("ConferencesWithNavbarStatus")]
        public async Task<ActionResult<IEnumerable<object>>> GetConferencesWithNavbarStatus()
        {
            try
            {
                var result = await _conferenceService.GetConferencesWithNavbarStatus();
                return Ok(result);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
