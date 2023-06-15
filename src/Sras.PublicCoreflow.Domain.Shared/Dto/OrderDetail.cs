namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class OrderDetail
    {
        public string ChargeType { get; set; } = string.Empty;
        public string? Option { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public double Subtotal { get; set; }
    }
}
