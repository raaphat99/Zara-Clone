namespace WebAPI.DTOs
{
    public class ProductvariantDTO
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public double DiscountPercentage { get; set; }
        public double DiscountedPrice { get; set; }
        public List<ProductImageDTO> ProductImages { get; set; } // All images for this variant
        public string ProductColors { get; set; }
        public List<ProductSizeDTO> AvailableSizes { get; set; }

    }
}
