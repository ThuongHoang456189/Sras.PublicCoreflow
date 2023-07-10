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

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Time")]
    [Route("api/sras/time")]
    public class TimeController : AbpController
    {
        private readonly ITimeAppService _appService;

        public TimeController(ITimeAppService appService)
        {
            _appService = appService;   
        }

        [HttpGet("current-time")]
        public ActionResult GetCurrentTime()
        {
            DateTime currentTime = DateTime.Now;
            return Ok(currentTime);
        }

        [HttpGet("change-time")]
        public ActionResult ChangeTime() {
            return Ok(_appService.ChangeSystemTime("11-06-2023"));
        }
            
    }

}

