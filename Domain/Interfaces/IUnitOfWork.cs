using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IProductImageRepository ProductImages { get; }
        IProductVariantRepository ProductVariant { get; }
        IUserAddressRepository UserAddress { get; }
        IUserMeasurementRepository UserMeasurements { get; }
        IUserRepository Users { get; }
        Task<int> Complete();
    }
}
