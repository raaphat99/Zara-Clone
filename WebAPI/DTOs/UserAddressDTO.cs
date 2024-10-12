namespace WebAPI.DTOs
{
    public class UserAddressDTO
    {

        public int Id { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public bool Active { get; set; } // Default address flag
        public string UserId { get; set; }


    }
}
