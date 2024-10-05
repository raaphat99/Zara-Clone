using Domain.Enums;

namespace WebAPI.DTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<string> ImageUrl { get; set; }
        public double Price { get; set; }
        public SizeValue Size { get; set; }
        public Color Color { get; set; }
        public int Quantity { get; set; }
    }
}
