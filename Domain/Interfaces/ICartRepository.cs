using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart, int>
    {
        Task RemoveProductVariantFromCart(string userId, int variantId);
        Task<User> GetUserWithCartItems(string userId);
    }
}
