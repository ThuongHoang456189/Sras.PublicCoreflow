using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("ConflictCase")]
    [Route("api/sras/conflictcases")]
    public class ConflictCaseController : AbpController
    {
        private readonly IConflictCaseAppService _conflictCaseService;
        public ConflictCaseController(IConflictCaseAppService conflictCaseService)
        {
            _conflictCaseService = conflictCaseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllConflictCases([Optional] Guid trackId)
        {
            try
            {
                var result = await _conflictCaseService.GetAllConflictCasesAsync(trackId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
