using Domain.Enums;

namespace WebAPI.DTOs
{
    public class OrderDTO
    {
        public string trackingNumber { get; set; } 
        public string created { get; set; } 
        public string status { get; set; } 
        public List<OrderItemDTO> items { get; set; } 
        public double totalPrice { get; set; }

    }

}




