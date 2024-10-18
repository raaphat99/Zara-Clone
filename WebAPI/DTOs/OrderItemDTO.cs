namespace WebAPI.DTOs
{
    public class OrderItemDTO
    {
        public string? name { get; set; } 
        public string? productImage { get; set; } 
        public int? quantity { get; set; } 
        public double? unitPrice { get; set; }
        public double? subtotal { get; set; }
        public string? color { get; set; }
        public string? size { get; set; }
    }
}
