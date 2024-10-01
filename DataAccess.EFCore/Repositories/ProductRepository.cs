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
    public class ProductRepository : GenericRepository<Product>//, IProductRepository
    {
        public ProductRepository(ApplicationContext applicationContext) : base(applicationContext)
        { }
        //public IEnumerable<Product> GetMostExpensiveProducts(int count)
        //{
        //    return _dbContext.Products.OrderByDescending(product => product.Price).Take(count).ToList();
        //}
    }
}
