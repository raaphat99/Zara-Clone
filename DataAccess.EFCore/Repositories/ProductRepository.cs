using DataAccess.EFCore.Data;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Color = Domain.Enums.Color;

namespace DataAccess.EFCore.Repositories
{
    public class ProductRepository : GenericRepository<Product, int>, IProductRepository
    {
        public ProductRepository(ApplicationContext applicationContext) : base(applicationContext)
        { }

        public async Task<ICollection<Product>> SearchProductsAsync(
            string searchTerm,
            string category = null,
            double? minPrice = null,
            double? maxPrice = null,
            string? size = null,
            Color? color = null,
            Material? material = null)
        {

            // Split the search query into individual keywords
            var keywords = searchTerm?.Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            IQueryable<Product> products = _dbSet.Include(prd => prd.ProductVariants).AsQueryable();

            // If there are keywords, search for them in various fields
            if (keywords.Length != 0)
            {
                foreach (var keyword in keywords)
                {
                    products = products.Where(p =>
                        p.Name.Contains(keyword) ||
                        p.Description.Contains(keyword) ||
                        p.Category.Name.Contains(keyword) ||
                        p.ProductVariants.Any(v => v.ProductMaterial.ToString() == keyword ||
                                                  v.ProductColor.ToString() == keyword));
                }
            }

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(prd => prd.Category.Name == category);
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                products = products.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.Price <= maxPrice.Value));
            }

            // Filter by color
            if (color.HasValue)
            {
                products = products.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.ProductColor == color.Value));
            }

            // Filter by material
            if (material.HasValue)
            {
                products = products.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.ProductMaterial == material.Value));
            }

            // Filter by size
            if (!string.IsNullOrEmpty(size))
            {
                products = products.Where(p => p.ProductVariants.Any(v => v.Size.Value.ToString() == size));
            }

            return await products.ToListAsync();
        }
    }
}
