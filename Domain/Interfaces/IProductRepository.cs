using Domain.Enums;
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
        Task<ICollection<Product>> SearchProductsAsync(
            string searchTerm,
            string? category = null,
            double? minPrice = null,
            double? maxPrice = null,
            string? size = null,
            Color? color = null,
            Material? material = null);

        IQueryable<Product> GetAllWithVariantsAndImages();
        IQueryable<ProductVariant> GetAllVariants();
        IQueryable<ProductVariant> GetVariantsByProductId(int productId);
    }


}
