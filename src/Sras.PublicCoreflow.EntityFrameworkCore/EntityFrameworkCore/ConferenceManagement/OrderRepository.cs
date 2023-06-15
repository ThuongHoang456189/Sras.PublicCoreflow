using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class OrderRepository : EfCoreRepository<PublicCoreflowDbContext, Email, Guid>, IOrderRepository
    {
        public OrderRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<object> GetOrderDetail(Guid orderId)
        {
            var dbContext = await GetDbContextAsync();
            var result = dbContext.Orders.Where(o => o.Id == orderId).First().OrderDetails;

            return JsonSerializer.Deserialize<OrderDto>(result);
        }
    }
}
