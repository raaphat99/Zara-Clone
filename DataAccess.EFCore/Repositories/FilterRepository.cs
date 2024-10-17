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
    public class FilterRepository : GenericRepository<Filter, int>, IFilterRepository
    {
        public FilterRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }
        public async Task<List<ProductVariant>> GetAllAsync()
        {
            return await _dbContext.ProductVariants.ToListAsync();
        }
        public async Task<IEnumerable<Filter>> GetFiltersByCategoryIdAsync(int categoryId)
        {
            return await _dbContext.Filters
                .Where(f => f.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}