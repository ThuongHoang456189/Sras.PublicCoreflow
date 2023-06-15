using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class OrderAppService : PublicCoreflowAppService, IOrderAppService
    {
        private readonly IOrderRepository orderRepository;

        public OrderAppService(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task<object> GetOrderDetail(Guid orderId)
        {
            return await orderRepository.GetOrderDetail(orderId);
        }
    }
}
