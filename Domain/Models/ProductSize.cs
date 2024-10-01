using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{

    public enum SizeType
    {
        Alpha,
        Numeric,
        AgeBased
    }

    public enum SizeValue
    {
        Small,
        Medium,
        Large,
        ExtraLarge,
        Size36 = 36,
        Size38 = 38,
        Size40 = 40,
        Size42 = 42,
        Size44 = 44
    }

    public enum TargetGroup
    {
        Men,
        Women,
        Kids
    }

    public class ProductSize
    {
        public int Id { get; set; }

        [ForeignKey("ProductType")]
        public int ProductTypeId { get; set; }
        public SizeType Type { get; set; }
        public SizeValue Value { get; set; }
        public TargetGroup Group { get; set; }
        public virtual ProductType ProductType { get; set; }

        public  virtual ICollection<ProductVariantSize> ProductVariantSize { get; set; } = new List<ProductVariantSize>();

    }
}
