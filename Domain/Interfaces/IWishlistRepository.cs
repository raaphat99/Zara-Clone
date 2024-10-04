using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWishlistRepository : IGenericRepository<Wishlist, int>
    {
        Task<Wishlist> AddToWishList(Wishlist wishList);

        Task RemoveFromWishList(Wishlist wishList);

        bool IsWishList(int itemId, string userId);
        Task<IEnumerable<Wishlist>> GetUserWishlist(string userId);
    }
}
