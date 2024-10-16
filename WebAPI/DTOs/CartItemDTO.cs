using Domain.Enums;

namespace WebAPI.DTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int? ProductVariantId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
    }
}
