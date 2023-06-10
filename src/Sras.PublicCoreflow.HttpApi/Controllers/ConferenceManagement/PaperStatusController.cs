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
    [ControllerName("PaperStatus")]
    [Route("api/sras/paper-statuses")]
    public class PaperStatusController : AbpController
    {
        private readonly IPaperStatusAppService _paperStatusAppService;

        public PaperStatusController(IPaperStatusAppService paperStatusAppService)
        {
            _paperStatusAppService = paperStatusAppService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllPaperStatus()
        {
            try
            {
                var result = await _paperStatusAppService.GetAllPaperStatusAsync();
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
