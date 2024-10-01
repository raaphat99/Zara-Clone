using DataAccess.EFCore.Data;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Color? color = null,
            Material? material = null)
        {
            var query = _dbSet.Include(prd => prd.ProductVariants).AsQueryable();

            // Search by name or description
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(prd => prd.Name.Contains(searchTerm) || prd.Description.Contains(searchTerm));
            }

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(prd => prd.Category.Name == category);
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.Price <= maxPrice.Value));
            }

            //// Filter by color
            if (color.HasValue)
            {
                query = query.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.ProductColor == color.Value));
            }

            // Filter by material
            if (material.HasValue)
            {
                query = query.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.ProductMaterial == material.Value));
            }

            return await query.ToListAsync();
        }
    }
}
