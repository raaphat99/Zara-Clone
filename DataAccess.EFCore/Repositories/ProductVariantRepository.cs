using DataAccess.EFCore.Data;
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
    public class ProductVariantRepository : GenericRepository<ProductVariant, int>, IProductVariantRepository
    {
        public ProductVariantRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }
        public async Task<List<ProductVariant>> GetAllAsync()
        {
            return await _dbContext.ProductVariants.ToListAsync();
        }

        // Get all product variants for a specific product
        public async Task<IEnumerable<ProductVariant>> GetVariantsByProductIdAsync(int productId)
        {
            return await _dbContext.ProductVariants
                                   .Include(v => v.Product)
                                   .Where(v => v.ProductId == productId)
                                   .ToListAsync();
        }

        // Get a specific product variant by its ID
        public async Task<ProductVariant> GetVariantByIdAsync(int variantId)
        {
            return await _dbContext.ProductVariants
                                   .Include(v => v.Product)
                                   .FirstOrDefaultAsync(v => v.Id == variantId);
        }

        // Add a new product variant
        public async Task AddVariantAsync(ProductVariant productVariant)
        {
            await _dbContext.ProductVariants.AddAsync(productVariant);
            await SaveChangesAsync(); // Save changes to the database
        }

        // Update an existing product variant
        public async Task UpdateVariantAsync(ProductVariant productVariant)
        {
            _dbContext.ProductVariants.Update(productVariant);
            await SaveChangesAsync(); // Save changes to the database
        }

        // Delete a product variant
        public async Task DeleteVariantAsync(ProductVariant productVariant)
        {
            _dbContext.ProductVariants.Remove(productVariant);
            await SaveChangesAsync(); // Save changes to the database
        }

        // Save changes asynchronously
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
