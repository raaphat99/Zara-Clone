using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductVariantRepository : IGenericRepository<ProductVariant, int>
    {
        Task<List<ProductVariant>> GetAllAsync(); // تأكد من أن هذا النوع يتطابق
        Task<IEnumerable<ProductVariant>> GetVariantsByProductIdAsync(int productId);
        Task<ProductVariant> GetVariantByIdAsync(int variantId);
        Task AddVariantAsync(ProductVariant productVariant);
        Task UpdateVariantAsync(ProductVariant productVariant);
        Task DeleteVariantAsync(ProductVariant productVariant);
    }
}
