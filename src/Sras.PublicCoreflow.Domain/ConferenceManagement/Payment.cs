using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Payment : FullAuditedAggregateRoot<Guid>
    {
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; }
        public int TotalWholeAmount { get; private set; }
        public int TotalFractionalAmount { get; private set; } = 0;
        public string Status { get; private set; }
        public string? PaymentProofRootFilePath { get; private set; }

        public Payment(Guid id, Guid orderId, int totalWholeAmount, int totalFractionalAmount, string status, string? paymentProofRootFilePath) : base(id)
        {
            OrderId = orderId;
            TotalWholeAmount = totalWholeAmount;
            TotalFractionalAmount = totalFractionalAmount;
            Status = status;
            PaymentProofRootFilePath = paymentProofRootFilePath;
        }
    }
}