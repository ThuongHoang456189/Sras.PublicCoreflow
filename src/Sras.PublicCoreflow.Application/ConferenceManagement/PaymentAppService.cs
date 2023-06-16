using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class PaymentAppService : PublicCoreflowAppService, IPaymentAppService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentAppService(IPaymentRepository paymentRepository)
        {
            this._paymentRepository = paymentRepository;
        }

        public async Task<object> CreatePaymentAsync(CreatePaymentRequest request)
        {
            return await _paymentRepository.CreatePaymentAsync(request);
        }
    }
}
