using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product, int>
    {
        Task<List<Product>> SearchProductsAsync(
            string searchTerm,
            string category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string color = null,
            string material = null);
    }
}
