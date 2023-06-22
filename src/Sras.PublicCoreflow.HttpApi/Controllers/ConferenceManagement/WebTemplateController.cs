using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("WebTemplate")]
    [Route("api/sras/web-templates")]
    public class WebTemplateController : AbpController
    {

        private readonly IWebTemplateAppService _webTemplateAppService;

        public WebTemplateController(IWebTemplateAppService webTemplateAppService)
        {
            _webTemplateAppService = webTemplateAppService;
        }

        //[HttpPost("web-template")]
        //public async Task<object> CreateWebTemplate([FromBody]string rootFilePath)
        //{
        //    try
        //    {
        //        var result = await _websiteAppService.CreateWebtemplate(rootFilePath);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet("{hasContent}")]
        public async Task<ActionResult<object>> GetNavbarOfWebsite(bool hasContent)
        {
            try
            {
                var result = await _webTemplateAppService.GetListWebTemplateName(hasContent);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("web-template-files")]
        public async Task<ActionResult<ResponseDto>> CreateWebTemplate([FromForm] List<RemoteStreamContent> file, string name, string description)
        {
            var result = await _webTemplateAppService.CreateTemplate(file.First(), name, description);
            return Ok(result);
        }

        [HttpGet("download-all-templates")]
        public ActionResult downloadAllTemplates()
        {
            var filedata = _webTemplateAppService.downloadAllTemplates();
            var filename = "webTemplate";

            return File(filedata.Result.First(), MediaTypeNames.Text.Plain, "WebTemplate.html");
        }

    }
}
