using Domain.Auth;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public WishListController(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems() 
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            var wishlist = user.Wishlist.Products.ToList();
            if(user == null) 
                return NotFound("User Not Found");
            if(!wishlist.Any())
                return NotFound("No Item In WishList");
            List<WishListItemDTO> wishListItemDTOs = new List<WishListItemDTO>();
            foreach (var item in wishlist)
            {
                WishListItemDTO wishListItemDTO = new WishListItemDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    ImageUrl = GetImgUrl(item)
                    
                };
                wishListItemDTOs.Add(wishListItemDTO);

            }
            return Ok(wishListItemDTOs);

        }

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> AddToWishList(int productId) 
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var wishlist =  await _unitOfWork.Wishlist.FindSingle(w=>w.UserId == userId);
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (wishlist != null && product != null)
            {
                var products = wishlist.Products.ToList();

                if (products.Contains(product))
                {
                    // Remove product from wishlist
                    wishlist.Products.Remove(product);
                    _unitOfWork.Wishlist.Update(wishlist);
                    await _unitOfWork.Complete();
                    return Ok(new Response { Status = "success", Message = "Item removed" });
                }
                else
                {
                    // Add product to wishlist
                    wishlist.Products.Add(product);
                    _unitOfWork.Wishlist.Update(wishlist);
                    await _unitOfWork.Complete();
                    return Ok(new Response { Status = "success", Message = "Item added" });
                }
            }
            return NotFound(new Response { Message = "Not Found" });
        }

        [HttpPost("move-to-cart/{wishlistItemId}")]
        public async Task<IActionResult> MoveToCart(int wishlistItemId)
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var wishlist = await _unitOfWork.Wishlist.FindSingle(w => w.UserId == userId);
            if (wishlist == null)
                return NotFound("Wishlist not found");

            var product = wishlist.Products.FirstOrDefault(p => p.Id == wishlistItemId);
            if (product == null)
                return NotFound("Product not found in wishlist");

            var cart = await _unitOfWork.Carts.FindSingle(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _unitOfWork.Carts.AddAsync(cart);
            }

            var productVariant = product.ProductVariants.FirstOrDefault();
            if (productVariant == null)
                return BadRequest("Product variant not available");

            // Check if the product is already in the cart
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductVariantId == productVariant.Id);
            if (cartItem != null)
            {
                // Increment the quantity if it's already in the cart
                cartItem.Quantity++;
                _unitOfWork.CartItems.Update(cartItem);
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductVariantId = productVariant.Id,
                    CartId = cart.Id,
                    Quantity = 1,
                    UnitPrice = productVariant.Price
                };
                await _unitOfWork.CartItems.AddAsync(cartItem);
            }

            wishlist.Products.Remove(product);
            _unitOfWork.Wishlist.Update(wishlist);

            await _unitOfWork.Complete();

            return Ok(new { Status = "success", Message = "Item moved to cart and removed from wishlist" });
        }


        private string GetImgUrl(Product product) 
        {
            var variants = product.ProductVariants.ToList();
            var imgs = variants[0].ProductImage.ToList();
            string imgUrl = imgs[0].ImageUrl;
            return imgUrl;
        }
    }
}
