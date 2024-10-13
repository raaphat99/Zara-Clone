using WebAPI.DTOs;

    public class OrderDetailsDTO
    {
        public string trackingNumber { get; set; }
        public DateTime orderDate { get; set; }
        public double totalPrice { get; set; }
        public double shippingCost { get; set; }
        public string paymentMethod { get; set; }
        public string status { get; set; }
        public CustomerDTO customer { get; set; }
        public List<OrderItemDTO> items { get; set; } = new List<OrderItemDTO>();
    }
