using Domain.Enums;

namespace WebAPI.DTOs.ProductDTOs
{
    public class ProductImageDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string? AlternativeText { get; set; }
        public int? SortOrder { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public string? ImageType { get; set; }
    }

}
