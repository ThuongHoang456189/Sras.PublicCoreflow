using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
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
    [ControllerName("Website")]
    [Route("api/sras/website")]
    public class WebsiteController : AbpController
    {

        private readonly IWebsiteAppService _websiteAppService;

        public WebsiteController(IWebsiteAppService websiteAppService)
        {
            _websiteAppService = websiteAppService;
        }

        [HttpPost("web-template")]
        public async Task<object> CreateWebTemplate([FromBody]string rootFilePath)
        {
            try
            {
                var result = await _websiteAppService.CreateWebtemplate(rootFilePath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{conferenceId}/{webTemplateId}")]
        public async Task<object> CreateWebsite(Guid webTemplateId, Guid conferenceId)
        {
            try
            {
                var result = await _websiteAppService.CreateWebsite(webTemplateId, conferenceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{conferenceId}")]
        public async Task<ActionResult<object>> GetNavbarOfWebsite(Guid conferenceId)
        {
            try
            {
                var result = await _websiteAppService.getNavbarByConferenceId(conferenceId);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{conferenceId}/{webTemplateId}/update-navbar")]
        public async Task<ActionResult<object>> UpdateNavbarOfWebsite(Guid conferenceId, Guid webTemplateId, [FromBody]NavbarDTO navbarDTO)
        {
            try
            {
                var result = await _websiteAppService.UpdateNavbarByConferenceId(conferenceId, webTemplateId, navbarDTO);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
