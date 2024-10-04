using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICartItemRepository : IGenericRepository<CartItem, int>
    {
        Task<List<CartItem>> GetCartItemsAsync(string userId);
       // int CheckAvailability(int productId, int sizeIndex);
        bool UpdateProductQuantity(string userId, int productId , int quantity , int sizeIndex);
        Task DeleteItem(string userId , int productId , int selectedSize);
        Task AddItemToCart(CartItem cartItem); 

    }
}
