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

        public async Task<Wishlist> AddToWishList(Wishlist wishListitem)
        {

            _dbContext.Wishlists.Add(wishListitem);
            await _dbContext.SaveChangesAsync();
            return wishListitem;
        }

       

        public async Task RemoveFromWishList(Wishlist wishlistitem)
        {
            var item = await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.UserId == wishlistitem.UserId && w.Products.Any(p => p.Id == wishlistitem.Products.First().Id));

            if (wishlistitem != null)
            {
                _dbContext.Wishlists.Remove(wishlistitem);
                await _dbContext.SaveChangesAsync();
            }

        }



        public bool IsWishList(int itemId, string userId)
        {
            return _dbContext.Wishlists.Any(w => w.UserId == userId && w.Products.Any(p => p.Id == itemId));

        }

        public async Task<IEnumerable<Wishlist>> GetUserWishlist(string userId)
        {
            return await _dbContext.Wishlists
                .Include(w => w.Products) 
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }


    }
}
