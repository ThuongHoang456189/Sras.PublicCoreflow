using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("ResearcherProfile")]
    [Route("api/sras/researcher-profile")]
    public class ResearcherProfileController : AbpController
    {
        private readonly IResearcherProfileAppService _appService;

        public ResearcherProfileController(IResearcherProfileAppService appService)
        {
            _appService = appService;   
        }

        [HttpGet("{userId}/hasResearcherProfile")]
        public async Task<bool> hasResearchProfile(Guid userId)
        {
            return await _appService.hasResearchProfile(userId);
        }

        [HttpGet("confirm-primary-email/{userId}/{email}")]
        public async Task<bool> confirmPrimaryEmail(Guid userId, string email)
        {
            return await _appService.confirmPrimaryEmail(userId, email);
        }

        [HttpGet("send-link-confirm-and-check-duplicate/{userId}/{email}")]
        public async Task<ActionResult<bool>> sendLinkConfirmAndCheckDuplicate(Guid userId, string email)
        {
            try
            {
                var result = await _appService.sendLinkConfirmAndCheckDuplicate(userId, email);
                return Ok(result);
            } catch (Exception ex)
            {
                if (ex.Message == "Duplicate Primary Email") return Forbid(ex.Message);
                return BadRequest(ex.Message);
            }
        }


    }

}

