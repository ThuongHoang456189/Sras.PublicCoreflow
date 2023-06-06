using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System.Threading.Tasks;
using System.Xml.Linq;
using System;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Outsider")]
    [Route("api/sras/outsiders")]
    public class OutsiderController : AbpController
    {
        private readonly IOutsiderAppService _outsiderService;

        public OutsiderController(IOutsiderAppService outsiderService)
        {
            _outsiderService = outsiderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllOutsiders()
        {
            try
            {
                var result = await _outsiderService.GetAllOutsider();
                return Ok(result);
            } catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<OutsiderCreateResponse>> CreateOutsiderAsync(OutsiderCreateRequest request)
        {
            try
            {
                var result = await _outsiderService.CreateOutsider(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<object> UpdateOutsider(OutsiderUpdateRequest request)
        {
            try
            {
                var result = await _outsiderService.UpdateOutsider(request);
                return Ok(result);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}