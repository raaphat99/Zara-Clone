using DataAccess.EFCore.Data;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Repositories
{
    class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }
    }
}
