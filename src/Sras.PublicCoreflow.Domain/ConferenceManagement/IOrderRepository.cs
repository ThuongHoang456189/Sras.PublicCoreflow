using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IOrderRepository
    {
        Task<object> GetOrderDetail(Guid orderId);
    }
}
