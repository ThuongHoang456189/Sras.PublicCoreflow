using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Email")]
    [Route("api/sras/emails")]
    public class EmailController : AbpController
    {
        private IEmailAppService _emailAppService;

        public EmailController(IEmailAppService emailAppService)
        {
            _emailAppService = emailAppService;
        }

        [HttpPost("send-email-each-status-in-submission")]
        public async Task<ActionResult<object>> SendEmailForEachStatus(PaperStatusToSendEmail request)
        {
            try
            {
                var result = await _emailAppService.SendEmailForEachStatus(request);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
