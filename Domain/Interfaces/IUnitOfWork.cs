using Microsoft.EntityFrameworkCore.Storage;
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
        IWishlistRepository Wishlist { get; }
        ISizeRepository Sizes { get; }
        ICartItemRepository CartItems { get; }
        INotificationRepository Notifications { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        ICartRepository Carts { get; }
        ITrackingNumberRepository TrackingNumbers { get; }
        IPaymentRepository Payments { get; }
        IFilterRepository Filters { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> Complete();
    }
}
