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
<<<<<<< HEAD
        IUserMeasurementRepository UserMeasurements { get; }
        IUserRepository Users { get; }
=======
        IProductSizeRepository ProductSize { get; }
>>>>>>> 2a5bd60dae99d1af3bfbace1d9124d7c53bb8c19
        Task<int> Complete();
    }
}
