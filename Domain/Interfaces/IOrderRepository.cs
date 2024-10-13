using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order, int>
    {
        public Task<Order> GetOrderByTrackingNumberAsync(string userId, string trackingNumber);

    }
}
