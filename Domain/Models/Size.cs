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
    public class Size
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SizeType")]
        public int? SizeTypeId { get; set; }
        public virtual SizeType SizeType { get; set; }
        public virtual ICollection<ProductVariant> ProductVariant { get; set; } = new List<ProductVariant>();
        public SizeValue Value { get; set; }
    }
}
