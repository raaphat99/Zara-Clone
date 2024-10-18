namespace WebAPI.DTOs
{
    public class CheckoutDTO
    {
       public IEnumerable<CheckoutItemDTO> cartItems { get; set; }
        public double totalPrice { get; set; }
        public string? shippingMethod { get; set; }
        public int? userAddressId { get; set; }
        public string? paymentMethod { get; set; }

    }
}
