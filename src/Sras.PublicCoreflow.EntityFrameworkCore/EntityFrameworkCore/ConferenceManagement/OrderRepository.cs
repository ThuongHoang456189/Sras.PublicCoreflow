﻿using Sras.PublicCoreflow.ConferenceManagement;
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
            var order = dbContext.Orders.Where(o => o.Id == orderId).First();
            var orderDetail = JsonSerializer.Deserialize<OrderDto>(order.OrderDetails);
            var acc = dbContext.Users.Where(u => u.Id == order.AccountId).First();
            var firstname = acc.Name;
            var lastname = acc.Surname;
            return new
            {
                orderDetail,
                firstname,
                lastname,
            };
        }

        public async Task<object> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var dbContext = await GetDbContextAsync();
            var paymentId = _guidGenerator.Create();
            if (!dbContext.Orders.Any(o => o.Id == request.orderId)) throw new Exception("OrderId not existing");
            Payment payment = new Payment(paymentId, request.orderId, (int)request.totalWholeAmount, 0, request.status, null);
            dbContext.Payments.Add(payment);
            dbContext.Orders.Where(o => o.Id == request.orderId).First().Payments.Add(payment);
            dbContext.SaveChanges();
            var afterAdded = dbContext.Payments.Where(p => p.Id == paymentId).First();
            return new
            {
                paymentId,
                afterAdded.TotalWholeAmount,
                afterAdded.Status,
                afterAdded.OrderId
            };
        }
    }
}
