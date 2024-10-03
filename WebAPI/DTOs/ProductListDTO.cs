namespace WebAPI.DTOs
{
    public class ProductListDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double DiscountPercentage { get; set; }
        public double DiscountedPrice { get; set; }
        public int StockQuantity { get; set; }
        public ProductImageDTO ProductImage { get; set; } // Thumbnail image
    }
}

