using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserMeasurement
    {
        [Key]
        public int Id { get; set; }
        public string MesurmentProfileName { get; set; }
        public string FavoriteSection { get; set; }
        public string SizeValue { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        public bool Active { get; set; } = false;
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
