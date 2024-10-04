using DataAccess.EFCore.Data;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Repositories
{
    public class CartItemRepository : GenericRepository<CartItem, int>, ICartItemRepository
    {
        private readonly UserManager<User> _userManager;
        public CartItemRepository(ApplicationContext applicationContext , UserManager<User> userManager) : base(applicationContext)
        {
            _userManager = userManager;
        }

        public async Task AddItemToCart(CartItem cartItem)
        {
            await _dbContext.AddAsync(cartItem);
            _dbContext.SaveChanges();
        }

        //public int CheckAvailability(int productId, int sizeIndex)
        //{
        //    var product = _dbContext.ProductVariants.FirstOrDefault(p => p.Id == productId);
        //    if (product != null)
        //    {
        //        SizeValue size;
        //        switch (sizeIndex)
        //        {
        //            case 0:
        //                size = SizeValue.Small;
        //                break;
        //            case 1:
        //                size = SizeValue.Medium;
        //                break;
        //            case 2:
        //                size = SizeValue.Large;
        //                break;
        //            case 3:
        //                size = SizeValue.ExtraLarge;
        //                break;
        //            default:
        //                throw new ArgumentException("Invalid size index");

        //        }

        //        var productwithsize = _dbContext.ProductVariants
        //            .Include(p => p.ProductSize).FirstOrDefault(p => p.Id == productId && p.ProductSize.Value == size && p.StockQuntity >= 0);
        //        return product;
        //    }

        //}

        public Task DeleteItem(string userId, int productId, int selectedSize)
        {
            throw new NotImplementedException();
        }

        public Task<List<CartItem>> GetCartItemsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateProductQuantity(string userId, int productId, int quantity, int sizeIndex)
        {
            SizeValue size;
            switch (sizeIndex)
            {
                case 0:
                    size = SizeValue.Small;
                    break;
                case 1:
                    size = SizeValue.Medium;
                    break;
                case 2:
                    size = SizeValue.Large;
                    break;
                case 3:
                    size = SizeValue.ExtraLarge;
                    break;
                default:
                    throw new ArgumentException("Invalid size index");

            }

            var cartitem = _dbContext.CartItems.FirstOrDefault(u => u.Cart.UserId == userId && u.ProductVariantId == productId && u.ProductVariant.ProductSize.Value == size);

            if (cartitem != null)
            {
                cartitem.Quantity = quantity;
                _dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
