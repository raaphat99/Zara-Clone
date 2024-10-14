namespace WebAPI.DTOs.ProductDTOs
{
    public class VariantForDetailsScreenDto
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public double DiscountPercentage { get; set; }
        public double DiscountedPrice { get; set; }
        public int StockQuantity { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public string ProductColor { get; set; }
        public string ProductMaterial { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
