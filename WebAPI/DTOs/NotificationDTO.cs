namespace WebAPI.DTOs
{
    public class NotificationDTO
    {
        public int id { get; set; }
        public string userId { get; set; }
        public string message { get; set; }
        public bool isRead { get; set; }
        public DateTime created { get; set; }
     
    }

}




