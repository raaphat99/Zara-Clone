namespace WebAPI.DTOs.ProductDTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public int? CategoryId { get; set; }
<<<<<<< HEAD
        public string? MainImageUrl { get; set; }
        public List<string>? DistinctColors { get; set; }
        public string? SizeType { get; set; }
        public List<string>? FilterName { get; set; } = new List<string>(); // List of filter names
=======
        public string MainImageUrl { get; set; }

>>>>>>> parent of a254992 (edit product)
    }
}
