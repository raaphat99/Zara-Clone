using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public enum ImageType
    {
        Main,
        Thumbnail,
        Gallery
    }

    public class ProductImage
    {
        public int Id { get; set; }
        [ForeignKey("ProductVariant")]
        public int ProductVariantId { get; set; }
        public string ImageUrl { get; set; }
        public string AlternativeText { get; set; }
        public int SortOrder { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public ImageType ImageType { get; set; }

        public virtual ProductVariant ProductVariant { get; set; }
    }
  
}
