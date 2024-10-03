namespace WebAPI.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ParentCategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? Description { get; set; }
    }
}
