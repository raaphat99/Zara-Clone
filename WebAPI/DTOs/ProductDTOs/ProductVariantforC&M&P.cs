using Domain.Enums;

namespace WebAPI.DTOs.ProductDTOs
{
    public class ProductVariantforC_M_P
    {
        public int Id { get; set; }

        public int productId { get; set; }
        public string? ProductName { get; set; }

        public int SizeId { get; set; }
        public string? SizeValue { get; set; }

        public double Price { get; set; }
        public int StockQuantity { get; set; }

        public Color ProductColor { get; set; }

        public Material ProductMaterial { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        // Collection of related product images
        //public List<ProductImageDto> ProductImages { get; set; } = new List<ProductImageDto>();

        // Optionally: Collection of related sizes for variants
        // public List<ProductVariantSizeDto> ProductVariantSizes { get; set; } = new List<ProductVariantSizeDto>();

    }
}
