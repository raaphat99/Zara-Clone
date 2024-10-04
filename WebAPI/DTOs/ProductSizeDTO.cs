using Domain.Enums;

namespace WebAPI.DTOs
{
    public class ProductSizeDTO
    {
        public int Id { get; set; }
        public Domain.Models.SizeType Type { get; set; }
        public SizeValue Value { get; set; }
    }
    public class ProductSizeDTOForAdd
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int Value { get; set; }
    }
}
