using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Time")]
    [Route("api/sras/time")]
    public class TimeController : AbpController
    {
        private readonly ITimeAppService _timeAppService;

        public TimeController(ITimeAppService timeAppService)
        {
            _timeAppService = timeAppService;
        }

        [HttpGet("now")]
        public IActionResult GetNow()
        {
            return Ok(_timeAppService.GetNow());
        }

        [HttpPost("now")]
        public async Task<IActionResult> SetNow(DateTime now)
        {
            return Ok(await _timeAppService.SetNow(now));
        }

        [HttpPost("now/reset")]
        public IActionResult Reset()
        {
            return Ok(_timeAppService.Reset());
        }
    }
}
