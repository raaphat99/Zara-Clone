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
    public class CartRepository : GenericRepository<Cart, int>, ICartRepository
    {
        public CartRepository(ApplicationContext applicationContext) : base(applicationContext)
        { }

        public async Task RemoveProductVariantFromCart(string userId, int variantId)
        {
            var cart = await _dbContext.Carts
                .Include(cart => cart.CartItems)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);

            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(cartItem => cartItem.Id == variantId);
                if (cartItem != null)
                {
                    cart.CartItems.Remove(cartItem);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<User> GetUserWithCartItems(string userId)
        {
            return await _dbContext.Users
                .Include(u => u.Cart)                       // Include the cart
                .ThenInclude(c => c.CartItems)              // Include cart items
                .ThenInclude(ci => ci.ProductVariant)       // Include product variant
                .ThenInclude(pv => pv.ProductImage)         // Include product images
                .FirstOrDefaultAsync(u => u.Id == userId);      // Filter by user ID
        }
    }
}
