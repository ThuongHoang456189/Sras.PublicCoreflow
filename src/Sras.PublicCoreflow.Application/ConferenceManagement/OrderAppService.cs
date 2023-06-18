using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class OrderAppService : PublicCoreflowAppService, IOrderAppService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderAppService(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }

        public async Task<object> GetOrderDetail(Guid orderId)
        {
            return await _orderRepository.GetOrderDetail(orderId);
        }

        
    }
}
