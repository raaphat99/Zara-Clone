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
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }
        public double Price { get; set; }
        public int StockQuntity { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Color ProductColor { get; set; }
        public Material ProductMaterial { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();
        public virtual ICollection<CartItem> CartItem { get; set; } = new List<CartItem>();
        public virtual ICollection<ProductImage> ProductImage { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
    }

}
