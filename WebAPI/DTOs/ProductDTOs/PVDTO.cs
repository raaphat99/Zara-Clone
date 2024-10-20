using Domain.Enums;

namespace WebAPI.DTOs.ProductDTOs
{
    public class PVDTO
    {
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public int Price { get; set; }
        public int StockQuantity { get; set; }
        public string ProductColor {  get; set; }
        public string ProductMaterial { get; set; }
        public int SizeId {  get; set; }
    }
}
