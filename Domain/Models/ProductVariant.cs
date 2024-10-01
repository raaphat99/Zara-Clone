using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public enum Color
    {
        AnimalPrint,
        Silver,
        Yellow,
        Orange,
        Purple,
        Red,
        Blue,
        Green,
        Black,
        White,
        Brown,
        Rosy,
        Ashen,
        Golden,
        Beige,
        Floor,
        Khaki,
        Reddish,
        Pleasant,
        Mineral
    }
    public enum Material
    {
        Cotton,
        Polyester,
        TheSkin,
        Rafia,
        Leather,
        Burlap,
        Lstered,
        Vinyl,
        Linen,
        Shiny,
        Sequins,
        TedSkin
    }

    public class ProductVariant
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public double Price { get; set; }
        public int StockQuntity { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Color ProductColor { get; set; }
        public Material ProductMaterial { get; set; }
        public int MyProperty { get; set; }
        public virtual Product Product { get; set; }
        // public virtual ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();
         //public virtual ICollection<CartItem> OrderItem { get; set; } = new List<CartItem>();

        public virtual ICollection<ProductImage> ProductImage { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductVariantSize> ProductVariantSize { get; set; } = new List<ProductVariantSize>();

    }
 
}
