using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Wishlist
    {
        [Key]
        public int ID { get; set; }
        public DateTime Created { get; set; }

        [ForeignKey("User")]
        public string? UserID { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Product> Products { get; set; }

    }
}
