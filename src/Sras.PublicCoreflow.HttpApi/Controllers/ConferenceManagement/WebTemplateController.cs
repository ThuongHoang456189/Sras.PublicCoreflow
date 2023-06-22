using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        public async Task<ActionResult<object>> GetNavbarOfWebsite()
        {
            try
            {
                var result = await _webTemplateAppService.GetListWebTemplateName();
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("web-template-files")]
        public ActionResult<ResponseDto> CreateWebTemplate([FromForm] List<RemoteStreamContent> file)
        {
            return Ok(_webTemplateAppService.CreateTemplate(file.First()));
        }


    }
}
