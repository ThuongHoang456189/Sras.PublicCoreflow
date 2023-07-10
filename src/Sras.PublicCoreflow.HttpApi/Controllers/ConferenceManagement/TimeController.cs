using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System.Xml.Linq;
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

        [HttpGet]
        public IActionResult GetNow()
        {
            return Ok(_timeAppService.GetTimeZone());
        }
    }
}
