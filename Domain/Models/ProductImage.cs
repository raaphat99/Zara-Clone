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


    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string? AlternativeText { get; set; }
        public int? SortOrder { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public ImageType? ImageType { get; set; }

        [ForeignKey("ProductVariant")]
        public int? ProductVariantId { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }

}
