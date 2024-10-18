namespace WebAPI.DTOs
{
    public class CheckoutDTO
    {
        public int? ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Subtotal { get; set; }
        public int? ShippingMethodId { get; set; }
        public int? UserAddressId { get; set; }
        public string PaymentMethod { get; set; }

    }
}
