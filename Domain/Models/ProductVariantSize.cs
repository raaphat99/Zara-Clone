using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ProductVariantSize
    {
        public int SizeId { get; set; }
        public int ProductVariantId { get; set; }
        public virtual ProductSize ProductSize { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}
