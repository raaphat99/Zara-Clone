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
    public class CategoryRepository: GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationContext applicationContext):base(applicationContext)
        {
            
        }
        public async Task<IEnumerable<Category>> GetSubCategoriesByParentIdAsync(int id)
        {
            return await _dbContext.Categories
                .Where(c => c.ParentCategoryId == id)
                .ToListAsync();
        }


    }
}
