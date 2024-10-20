using DataAccess.EFCore.Data;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Fields
        private readonly ApplicationContext _context;
        private readonly Lazy<IProductRepository> products;
        private readonly Lazy<IProductVariantRepository> productVariant;
        private readonly Lazy<ICategoryRepository> categories;
        private readonly Lazy<IProductImageRepository> productImages;
        private readonly Lazy<IUserAddressRepository> userAddress;
        private readonly Lazy<IWishlistRepository> wishlist;
        private readonly Lazy<IUserMeasurementRepository> userMeasurements;
        private readonly Lazy<IUserRepository> users;
        private readonly Lazy<ISizeRepository> sizes;
        private readonly Lazy<ICartItemRepository> cartItems;
        private readonly Lazy<INotificationRepository> notifications;
        private readonly Lazy<IOrderRepository> order;
        private readonly Lazy<IOrderItemRepository> orderItem;
        private readonly Lazy<ICartRepository> carts;
        private readonly Lazy<ITrackingNumberRepository> trackingNumbers;
        private readonly Lazy<IPaymentRepository> payments;
        private readonly Lazy<IFilterRepository> filters;
        private readonly Lazy<IProductTypeRepository> sizeType;


        #endregion


        #region Constructor
        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            // Lazy<T> class is used to defer the creation of the repositories until they are accessed.
            productVariant = new Lazy<IProductVariantRepository>(() => new ProductVariantRepository(_context));
            products = new Lazy<IProductRepository>(() => new ProductRepository(_context));
            categories = new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));
            productImages = new Lazy<IProductImageRepository>(() => new ProductImageRepository(_context));
            userAddress = new Lazy<IUserAddressRepository>(() => new UserAddressRepository(_context));
            wishlist = new Lazy<IWishlistRepository>(() => new WishlistRepository(_context));
            userMeasurements = new Lazy<IUserMeasurementRepository>(() => new UserMeasurementRepository(_context));
            users = new Lazy<IUserRepository>(() => new UserRepository(_context));
            sizes = new Lazy<ISizeRepository>(() => new SizeRepository(_context));
            cartItems = new Lazy<ICartItemRepository>(() => new CartItemRepository(_context));
            notifications = new Lazy<INotificationRepository>(() => new NotificationRepository(_context));
            order = new Lazy<IOrderRepository>(() => new OrderRepository(_context));
            orderItem = new Lazy<IOrderItemRepository>(() => new OrderItemRepository(_context));
            carts = new Lazy<ICartRepository>(() => new CartRepository(_context));
            trackingNumbers = new Lazy<ITrackingNumberRepository>(() => new TrackingNumberRepository(_context));
            payments = new Lazy<IPaymentRepository>(() => new PaymentRepository(_context));
            filters = new Lazy<IFilterRepository>(() => new FilterRepository(_context));

            sizeType=new Lazy<IProductTypeRepository>(() => new ProductTypeRepository(_context));

        }
        #endregion


        #region Getters
        //The Value property of Lazy<T> ensures that the repository is instantiated only once and then reused. (Singleton object)
        public IProductRepository Products => products.Value;
        public IProductVariantRepository ProductVariant => productVariant.Value;
        public ICategoryRepository Categories => categories.Value;
        public IProductImageRepository ProductImages => productImages.Value;
        public IUserAddressRepository UserAddress => userAddress.Value;
        public IWishlistRepository Wishlist => wishlist.Value;
        public IUserMeasurementRepository UserMeasurements => userMeasurements.Value;
        public IUserRepository Users => users.Value;
        public ISizeRepository Sizes => sizes.Value;
        public ICartItemRepository CartItems => cartItems.Value;
        public INotificationRepository Notifications => notifications.Value;
        public IOrderRepository Orders => order.Value;
        public IOrderItemRepository OrderItems => orderItem.Value;
        public ICartRepository Carts => carts.Value;
        public ITrackingNumberRepository TrackingNumbers => trackingNumbers.Value;
        public IPaymentRepository Payments => payments.Value;
        public IFilterRepository Filters => filters.Value;
        public IProductTypeRepository SizeType => sizeType.Value;


        #endregion


        #region Methods
        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

      
   
    public void Dispose()
        {
            _context.Dispose();
        }
        #endregion
    }
}
