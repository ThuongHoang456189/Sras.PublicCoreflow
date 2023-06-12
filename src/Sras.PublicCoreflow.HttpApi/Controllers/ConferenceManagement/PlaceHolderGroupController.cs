using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("PlaceHolderGroup")]
    [Route("api/sras/place-holder-groups")]
    public class PlaceHolderGroupController : AbpController
    {
        private readonly IPlaceHolderGroupAppService _appService;

        public PlaceHolderGroupController(IPlaceHolderGroupAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllSupportedPlaceHolder()
        {
            try
            {
                var result = await _appService.GetAllSupportedPlaceHolderAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
