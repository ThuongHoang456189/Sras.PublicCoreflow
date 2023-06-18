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
            return await _websiteAppService.CreateWebtemplate(rootFilePath);
        }

        [HttpPost("{conferenceId}/{webTemplateId}")]
        public async Task<object> CreateWebsite(Guid webTemplateId, Guid conferenceId)
        {
            return await _websiteAppService.CreateWebsite(webTemplateId, conferenceId);
        }

        [HttpGet("{conferenceId}")]
        public async Task<object> GetNavbarOfWebsite(Guid conferenceId)
        {
            return await _websiteAppService.getNavbarByConferenceId(conferenceId);
        }

    }
}
