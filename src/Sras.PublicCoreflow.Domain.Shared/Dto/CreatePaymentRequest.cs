using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class CreatePaymentRequest
    {
        public Guid orderId { get; set; }
        public float totalWholeAmount { get; set; }
        public string status { get; set; }
    }
}
