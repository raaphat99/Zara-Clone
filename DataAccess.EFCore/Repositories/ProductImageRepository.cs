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
    public class ProductImageRepository : GenericRepository<ProductImage, int>, IProductImageRepository
    {
        private readonly ApplicationContext _context;

        public ProductImageRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
            _context = applicationContext;
        }

        public async Task<IEnumerable<ProductImage>> GetImagesByVariantIdAsync(int variantId)
        {
            return await _context.ProductImages
            .Where(img => img.ProductVariantId == variantId)
            .ToListAsync();
        }
    }
}
