using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetAllItems(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            var cart = user.Cart.CartItems.ToList();
            // if(cart == null )
            //return NotFound("User has No Cart");
            if (!cart.Any())
                return NotFound("No Items In This Cart");
            List<CartItemDTO> cartItems = new List<CartItemDTO>();
            foreach (var item in cart)
            {
                var images = item.ProductVariant.ProductImage.ToList();
                var cartItemDTO = new CartItemDTO
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    Color = item.ProductVariant.ProductColor,
                    Size = item.ProductVariant.Size.Value,
                    Title = item.ProductVariant.Product.Name,
                    ImageUrl = await GetImgUrls(item),
                    Price = item.UnitPrice

                };
                cartItems.Add(cartItemDTO);
            }

            return Ok(cartItems);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCartItem(int productVariantId)
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            Cart cart = await _unitOfWork.Carts.FindSingle(c => c.UserId == userId);
            List<CartItem> cartitems = cart.CartItems.ToList();
            ProductVariant variant = await _unitOfWork.ProductVariant.GetByIdAsync(productVariantId);
            if (variant.StockQuantity == 0)
                return NotFound("Out Of Stock");
            bool exist = false;
            int itemId = 0;
            foreach (var item in cartitems)
            {
                if (item.ProductVariant.Id == variant.Id && item.ProductVariant.SizeId == variant.SizeId)
                {
                    exist = true;
                    itemId = item.Id;
                    break;
                }

            }
            CartItem newitem = new CartItem();

            if (!exist && variant.StockQuantity > 0)
            {
                newitem = new CartItem()
                {
                    Quantity = 1,
                    ProductVariantId = variant.Id,
                    CartId = user.Cart.Id,
                    UnitPrice = variant.Price
                };
                await _unitOfWork.CartItems.AddAsync(newitem);
                await _unitOfWork.Complete();
                return Ok("Item added");
            }
            else if (exist && variant.StockQuantity > 0)
            {
                var item = await _unitOfWork.CartItems.GetByIdAsync(itemId);
                item.Quantity++;
                _unitOfWork.CartItems.Update(item);
                await _unitOfWork.Complete();
                return Ok("Item Quantity Increased");

            }
            else
            {
                return NotFound("Out Of Stock");
            }

        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveItemFromCart(int cartItemId)
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            await _unitOfWork.Carts.RemoveProductVariantFromCart(userId, cartItemId);
            return Ok(new { message = "Item removed from cart." });
        }

        private async Task<ICollection<string>> GetImgUrls(CartItem cart)
        {
            var item = await _unitOfWork.CartItems.GetByIdAsync(cart.Id);
            var imgs = item.ProductVariant.ProductImage.ToList();
            List<string> urls = new List<string>();
            foreach (var i in imgs)
            {
                urls.Add(i.ImageUrl);
            }
            return urls;
        }
    }
}
