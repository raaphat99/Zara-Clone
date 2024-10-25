using Domain.Enums;

namespace WebAPI.DTOs
{
    public class SizeDTO
    {
        public int Id { get; set; }
        public string? SizeType { get; set; } // Alpha | Numeric | Age-Based
        public SizeValue Value { get; set; }
        public string? sizevalue { get; set; }
        public string? stringifiedValue { get; set; } // Small | Medium | OneYear | 36 and so on
    }
}
