using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Order : FullAuditedAggregateRoot<Guid>
    {
        public Guid AccountId { get; private set; }
        public IdentityUser Account { get; private set; }
        public string OrderDetails { get; private set; }
        public int TotalWholeAmount { get; private set; }
        public int TotalFractionalAmount { get; private set; } = 0;
        public string Currency { get; private set; }

        public ICollection<Payment> Payments { get; private set; }

        public Order(Guid id, Guid accountId, string orderDetails, int totalWholeAmount, int totalFractionalAmount, string currency) : base(id)
        {
            AccountId = accountId;
            OrderDetails = orderDetails;
            TotalWholeAmount = totalWholeAmount;
            TotalFractionalAmount = totalFractionalAmount;
            Currency = currency;

            Payments = new Collection<Payment>();
        }
    }
}
