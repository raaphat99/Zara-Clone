namespace WebAPI.DTOs.ProductDTOs
{
    public class filterDto
    {
        public int filterid { get; set; }
        public List<string> FilterName { get; set; }=new List<string>();
    }
}
