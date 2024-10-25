﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public virtual Category ParentCategory { get; set; }

        [ForeignKey("SizeType")]
        public int? SizeTypeId { get; set; }
        public virtual SizeType SizeType { get; set; }
        public virtual ICollection<Category> Subcategories { get; set; } = new List<Category>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Filter> Filters { get; set; } = new List<Filter>();
    }
}
