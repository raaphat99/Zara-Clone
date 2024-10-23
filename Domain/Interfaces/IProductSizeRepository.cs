using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISizeRepository : IGenericRepository<Size, int>
    {
        Task<IEnumerable<Size>> GetSizesByType(Domain.Models.SizeType sizeType);
        Task<Size> GetSizeByValue(SizeValue sizeValue);
        IEnumerable<Size> GetSizesByVariantId(int variantId);
    }
}
