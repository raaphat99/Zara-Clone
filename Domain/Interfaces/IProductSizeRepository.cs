using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductSizeRepository : IGenericRepository<ProductSize, int>
    {
        Task<IEnumerable<ProductSize>> GetSizesByType(Domain.Models.SizeType sizeType);
        Task<ProductSize> GetSizeByValue(SizeValue sizeValue);
    }
}
