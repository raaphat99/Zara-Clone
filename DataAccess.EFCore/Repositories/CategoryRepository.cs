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
    public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(ApplicationContext applicationContext) : base(applicationContext)
        { }

        // checks whether the given category or any of its ancestors is "Beauty".
        // The recursion stops when there’s no parent or the "Beauty" category is found.
        public async Task<IEnumerable<Category>> GetSubCategoriesByParentIdAsync(int id)
        {
            return await _dbContext.Categories
                .Where(c => c.ParentCategoryId == id)
                .ToListAsync();
        }

        public async Task<bool> IsBeautyAncestor(int categoryId)
        {
            var category = await _dbContext.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return false;
            }

            // Check if this category or any parent is "Beauty"
            return await CheckBeautyAncestor(category);
        }

        public async Task<bool> CheckBeautyAncestor(Category category)
        {
            if (category.Name == "BEAUTY")
            {
                return true;
            }

            if (category.ParentCategory == null)
            {
                return false;
            }

            // Recursively check parent
            return await IsBeautyAncestor(category.ParentCategory.Id);
        }

    }
}
