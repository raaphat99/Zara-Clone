using Domain.Enums;

namespace WebAPI.DTOs
{
    public class SizeDTO
    {
        public int Id { get; set; }
        public string? SizeType { get; set; } // Assuming you need a readable name from SizeType
        public SizeValue Value { get; set; }
    }
}
