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
    [ControllerName("Payment")]
    [Route("api/sras/payments")]
    public class PaymentController : AbpController
    {
        private readonly IPaymentAppService _paymentAppService;

        public PaymentController(IPaymentAppService paymentAppService)
        {
            _paymentAppService = paymentAppService;
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreatePaymentAsync(CreatePaymentRequest request)
        {
            try
            {
                var result = await _paymentAppService.CreatePaymentAsync(request);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}