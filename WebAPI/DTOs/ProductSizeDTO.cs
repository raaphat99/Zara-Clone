using Domain.Enums;

namespace WebAPI.DTOs
{
    public class ProductSizeDTO
    {
        public int Id { get; set; }
        public SizeType Type { get; set; }
        public SizeValue Value { get; set; }
    }
}
