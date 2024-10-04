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
    public class ProductSizeRepository : GenericRepository<ProductSize, int>, IProductSizeRepository
    {
        public ProductSizeRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }
        // Existing methods
        public async Task<IEnumerable<ProductSize>> GetSizesByType(Domain.Models.SizeType sizeType)
        {
            return await _dbContext.ProductSizes
                .Where(size => size.SizeType == sizeType)
                .ToListAsync();
        }

        // Implementing GetSizeByValue method
        public async Task<ProductSize> GetSizeByValue(SizeValue sizeValue)
        {
            return await _dbContext.ProductSizes
                .FirstOrDefaultAsync(size => size.Value == sizeValue);
        }


    }
}
