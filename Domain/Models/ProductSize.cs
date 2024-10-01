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
    public class ProductSize
    {
        [Key]
        public int Id { get; set; }
        public SizeType Type { get; set; }
        public SizeValue Value { get; set; }
        public TargetGroup Group { get; set; }

        [ForeignKey("ProductType")]
        public int? ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    }
}
