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
    public class OrderRepository : GenericRepository<Order, int>, IOrderRepository
    {
        private readonly ApplicationContext _context;

        public OrderRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
            _context = applicationContext;
        }

        public async Task<Order> GetOrderByTrackingNumberAsync(string userId, string trackingNumber)
        {
            return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                    .ThenInclude(pv => pv.ProductImage)
            .Include(o => o.User)
            .Include(o => o.ShippingMethod)
            .Include(o => o.Payment)
            .Include(o => o.User.Adresses)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.TrackingNumber == trackingNumber);
        }
    }
}
