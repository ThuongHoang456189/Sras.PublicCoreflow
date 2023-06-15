using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class PriceTable
    {
        public int MaxValidNumberOfPages { get; set; }
        public bool IsEarlyRegistrationEnabled { get; set; }
        public DateTime? EarlyRegistrationDeadline { get; set; }
        public int MaxNumberOfExtraPapers { get; set; }
        public List<PriceTableRow>? Rows { get; set; }
    }
}