using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category, int>
    {
        Task<IEnumerable<Category>> GetSubCategoriesByParentIdAsync(int id);
        Task<bool> IsBeautyAncestor(int categoryId);
        Task<bool> CheckBeautyAncestor(Category category);
    }
}
