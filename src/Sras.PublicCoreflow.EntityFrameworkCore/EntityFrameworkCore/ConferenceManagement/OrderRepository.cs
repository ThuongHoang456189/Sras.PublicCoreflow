using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class OrderRepository : EfCoreRepository<PublicCoreflowDbContext, Order, Guid>, IOrderRepository
    {
        private IGuidGenerator _guidGenerator;
        public OrderRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<object> GetOrderDetail(Guid orderId)
        {
            var dbContext = await GetDbContextAsync();
            var result = dbContext.Orders.Where(o => o.Id == orderId).First().OrderDetails;

            return JsonSerializer.Deserialize<OrderDto>(result);
        }
    
    }
}
