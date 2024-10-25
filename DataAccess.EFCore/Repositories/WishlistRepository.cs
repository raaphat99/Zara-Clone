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
    public class WishlistRepository : GenericRepository<Wishlist, int>, IWishlistRepository
    {
        public WishlistRepository(ApplicationContext applicationContext) : base(applicationContext)
        { }

        public async Task AddProductToWishlist(string userId, int productId)
        {
            // Find the user's wishlist and include the Products collection
            var wishlist = await _dbContext.Wishlists
                .Include(w => w.Products)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist != null)
            {
                // Check if the product is already in the wishlist
                if (!wishlist.Products.Any(p => p.Id == productId))
                {
                    var product = await _dbContext.Products.FindAsync(productId);
                    if (product != null)
                    {
                        // Add the product to the Products collection
                        wishlist.Products.Add(product);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            else
            {
                // If the user doesn't have a wishlist yet, create one and add the product
                var newWishlist = new Wishlist
                {
                    UserId = userId,
                    Products = new List<Product>()
                };

                var product = await _dbContext.Products.FindAsync(productId);
                if (product != null)
                {
                    newWishlist.Products.Add(product);
                    _dbContext.Wishlists.Add(newWishlist);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveProductFromWishlist(string userId, int productId)
        {
            var wishlist = await _dbContext.Wishlists
                .Include(w => w.Products)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist != null)
            {
                var product = wishlist.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    wishlist.Products.Remove(product);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<User> GetUserWithWishlist(string userId)
        {
            return await _dbContext.Users.Include(u => u.Wishlist).FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
