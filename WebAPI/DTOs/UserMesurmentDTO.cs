using Domain.Enums;

namespace WebAPI.DTOs
{
    public class UserMesurmentDTO
    {
        public int? Id { get; set; }
        public string MesurmentProfileName { get; set; }
        public string FavoriteSection { get; set; }
        public string SizeValue { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        
    }
}
