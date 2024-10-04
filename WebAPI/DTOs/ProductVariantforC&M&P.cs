using Domain.Enums;

namespace WebAPI.DTOs
{
    public class ProductVariantforC_M_P
    {
        public int Id { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        // Foreign key for Size
        public int SizeId { get; set; }
        public string SizeValue { get; set; }

        // Pricing and stock details
        public double Price { get; set; }
        public int StockQuantity { get; set; }

        // Enum for color
        public Color ProductColor { get; set; }

        // Enum for material
        public Material ProductMaterial { get; set; }

        // Timestamps for record creation and updates
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        // Collection of related product images
        //public List<ProductImageDto> ProductImages { get; set; } = new List<ProductImageDto>();

        // Optionally: Collection of related sizes for variants
       // public List<ProductVariantSizeDto> ProductVariantSizes { get; set; } = new List<ProductVariantSizeDto>();

    }
}
