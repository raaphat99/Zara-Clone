using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        [ForeignKey("ProductVariant")]
        public int? ProductVariantId { get; set; }
        public virtual ProductVariant ProductVariant { get; set; } 

        [ForeignKey("Cart")]
        public int? CartId { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
