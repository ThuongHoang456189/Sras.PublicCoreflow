using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

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

        [HttpGet("get-navbar/{conferenceId}")]
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
        public async Task<ActionResult<object>> UpdateNavbarOfWebsite(Guid conferenceId, Guid webTemplateId, [FromBody] NavbarDTO navbarDTO)
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

        [HttpPost("add-content-of-website/{conferenceId}/{fileName}")]
        public async Task<ActionResult<object>> AddContentOfWebsite(Guid conferenceId, string fileName, [FromBody] ContentBodyRequest content)
        {
            try
            {
                _websiteAppService.UploadContentOfWebsite(conferenceId, fileName, content.temp, content.finalRuslt);
                return Ok(new ResponseDto()
                {
                    IsSuccess = true
                });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-content-temp-file/{conferenceId}/{fileName}")]
        public async Task<object> GetContentTempOfWebsite(Guid conferenceId, string fileName)
        {
            try
            {
                var result = _websiteAppService.GetContentTempOfWebsite(conferenceId, fileName);
                return Ok(result);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IEnumerable<object>> GetAllWebsite()
        {
            return await _websiteAppService.GetAllWebsite();
        }

    }
}
