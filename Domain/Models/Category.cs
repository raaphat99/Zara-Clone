using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Category
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryID { get; set; }
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> Subcategories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
