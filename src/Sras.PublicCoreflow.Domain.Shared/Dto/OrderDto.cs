using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class OrderDto
    {
        public List<OrderDetail> Details { get; set; } = new List<OrderDetail>();
        public double Total { get; set; }
    }
}
