using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SizeType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual SizeTypes Type { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Size> Sizes { get; set; } = new List<Size>();
    }
}
