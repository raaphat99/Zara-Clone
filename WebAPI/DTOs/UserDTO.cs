namespace WebAPI.DTOs
{
    public class UserDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public UserAddressDTO? ActiveAddress { get; set; }
        public string? ActiveMesurment { get; set; }
        public IEnumerable<OrderDTO>? Orders { get; set; }
    }
}
