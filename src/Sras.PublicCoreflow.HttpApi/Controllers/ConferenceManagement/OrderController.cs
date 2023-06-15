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
    [ControllerName("Order")]
    [Route("api/sras/orders")]
    public class OrderController : AbpController
    {
        private readonly IOrderAppService _orderAppService;

        public OrderController(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<object>> GetOrderDetail(Guid orderId)
        {
            try
            {
                var result = await _orderAppService.GetOrderDetail(orderId);
                return Ok(result);
            } catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
    }
}