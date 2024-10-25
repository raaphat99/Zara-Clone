namespace WebAPI.DTOs.ProductDTOs
{
    public class ProductDetailDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double DiscountPercentage { get; set; }
        public double DiscountedPrice { get; set; }
        public List<ProductVariantDTO> ProductVariants { get; set; }

    }

}
