using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("SrasBackground")]
    [Route("api/sras/background")]
    public class SrasBackgroundController : AbpController
    {
        private readonly ISrasBackgroundAppService _srasBackgroundAppService;

        public SrasBackgroundController(ISrasBackgroundAppService srasBackgroundAppService)
        {
            _srasBackgroundAppService = srasBackgroundAppService;
        }

        [HttpGet("activity-timeline")]
        public async Task<IActionResult> UpdateActivityTimelineAsync()
        {
            try
            {
                await _srasBackgroundAppService.UpdateActivityTimelineAsync();
                return Ok("Completed successfully");
            }
            catch (Exception)
            {
                return BadRequest("An error occurred");
            }
        }
    }
}
