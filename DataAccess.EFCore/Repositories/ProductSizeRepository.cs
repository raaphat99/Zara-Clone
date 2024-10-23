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
    public class SizeRepository : GenericRepository<Size, int>, ISizeRepository
    {
        public SizeRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        // Existing methods
        public async Task<IEnumerable<Size>> GetSizesByType(Domain.Models.SizeType sizeType)
        {
            return await _dbContext.Sizes
                .Where(size => size.SizeType == sizeType)
                .ToListAsync();
        }

        // Implementing GetSizeByValue method
        public async Task<Size> GetSizeByValue(SizeValue sizeValue)
        {
            return await _dbContext.Sizes
                .FirstOrDefaultAsync(size => size.Value == sizeValue);
        }
        public async Task<List<Size>> GetAllSizesAsync()
        {
            return await _dbContext.Sizes.ToListAsync();
        }

        public IEnumerable<Size> GetSizesByVariantId(int variantId)
        {
            return _dbContext.Sizes
                           .Include(s => s.ProductVariant)
                           .Where(s => s.ProductVariant.Any(pv => pv.Id == variantId))
                           .ToList();
        }



    }
}
