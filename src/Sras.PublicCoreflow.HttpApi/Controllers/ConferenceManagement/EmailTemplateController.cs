﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
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
    [ControllerName("EmailTemplate")]
    [Route("api/sras/email-teamplates")]
    public class EmailTemplateController : AbpController
    {
        private readonly IEmailTemplateAppService _emailTemplateAppService;

        public EmailTemplateController(IEmailTemplateAppService emailTemplateAppService) {
            _emailTemplateAppService = emailTemplateAppService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetEmailTemplateById(Guid id) {
            try
            {
                var result = await _emailTemplateAppService.GetEmailTemplateById(id);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}