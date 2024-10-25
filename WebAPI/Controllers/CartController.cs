using Domain.Auth;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllItems()
        {
            // Extract the user ID from the JWT token
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;

            if (userId == null)
                return BadRequest("User ID not found.");

            // Load user and include CartItems and related ProductVariant and ProductImage data
            var user = await _unitOfWork.Carts.GetUserWithCartItems(userId);

            if (user == null || user.Cart == null)
                return NotFound("User or Cart not found.");

            // Get the cart items
            var cartItems = user.Cart.CartItems.ToList();

            if (!cartItems.Any())
                return NotFound("No Items In This Cart");

            // Prepare the DTO list
            List<CartItemDTO> cartItemDTOs = new List<CartItemDTO>();

            foreach (var item in cartItems)
            {
                // Convert images to URL (you may need to implement the method)
                var imageUrl = GetImgUrl(item);

                var cartItemDTO = new CartItemDTO
                {
                    Id = item.Id,
                    ProductVariantId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    Color = item.ProductVariant.ProductColor.ToString(),
                    Size = item.ProductVariant.Size.Value.ToString(),
                    Title = item.ProductVariant.Product.Name,
                    ImageUrl = imageUrl,
                    Price = item.UnitPrice
                };

                cartItemDTOs.Add(cartItemDTO);
            }

            return Ok(cartItemDTOs);
        }


        // Helper method to get the first image URL or a default placeholder
        private string GetImgUrl(CartItem item)
        {
            var productImages = item.ProductVariant?.ProductImage?.ToList();

            if (productImages != null && productImages.Any())
            {
                // Assuming you have a property for the image URL
                return productImages.FirstOrDefault()?.ImageUrl ?? "https://picsum.photos/seed/picsum/200/300";
            }

            return "https://picsum.photos/seed/picsum/200/300"; // Return a default image URL if no images are available
        }


        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> GetAllItems()
        //{
        //    string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
        //    var user = await _unitOfWork.Users.GetByIdAsync(userId);
        //    var cart = user.Cart.CartItems.ToList();
        //    // if(cart == null )
        //    //return NotFound("User has No Cart");
        //    if (!cart.Any())
        //        return NotFound("No Items In This Cart");
        //    List<CartItemDTO> cartItems = new List<CartItemDTO>();
        //    foreach (var item in cart)
        //    {
        //        var images = item.ProductVariant.ProductImage.ToList();
        //        var cartItemDTO = new CartItemDTO
        //        {
        //            Id = item.Id,
        //            ProductVariantId = item.ProductVariantId,
        //            Quantity = item.Quantity,
        //            Color = item.ProductVariant.ProductColor.ToString(),
        //            Size = item.ProductVariant.Size.Value.ToString(),
        //            Title = item.ProductVariant.Product.Name,
        //            ImageUrl = GetImgUrl(item),
        //            Price = item.UnitPrice

        //        };
        //        cartItems.Add(cartItemDTO);
        //    }

        //    return Ok(cartItems);

        //}

        [HttpPost("{productVariantId:int}")]
        public async Task<IActionResult> AddCartItem(int productVariantId)
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            Cart cart = await _unitOfWork.Carts.FindSingle(c => c.UserId == userId);
            List<CartItem> cartitems = cart.CartItems.ToList();
            ProductVariant product = await _unitOfWork.ProductVariant.GetByIdAsync(productVariantId);
            if (product.StockQuantity == 0)
                return NotFound("Out Of Stock");
            bool exist = false;
            int itemId = 0;
            foreach (var item in cartitems)
            {
                if (item.ProductVariant.Id == product.Id && item.ProductVariant.SizeId == product.SizeId)
                {
                    exist = true;
                    itemId = item.Id;
                    break;
                }

            }
            CartItem newitem = new CartItem();
            if (!exist && product.StockQuantity > 0)
            {
                newitem = new CartItem()
                {
                    Quantity = 1,
                    ProductVariantId = product.Id,
                    CartId = user.Cart.Id,
                    UnitPrice = product.Price
                };
                await _unitOfWork.CartItems.AddAsync(newitem);
                await _unitOfWork.Complete();
                return Ok(new Response { Status = "success", Message = "Item Added" });
            }
            else if (exist && product.StockQuantity > 0)
            {
                var item = await _unitOfWork.CartItems.GetByIdAsync(itemId);
                if (item.Quantity < product.StockQuantity)
                {
                    item.Quantity++;
                    _unitOfWork.CartItems.Update(item);
                    await _unitOfWork.Complete();
                    return Ok(new Response { Status = "success", Message = "Quantity Increased" });

                }
                else
                {

                    return NotFound(new Response { Status = "Failed", Message = "Out Of Stock" });

                }

               

            }
            else
            {
                return Ok(new Response { Status = "Failed", Message = "Out Of Stock" });
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


        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DecreaseOrRemoveItemFromCart(int cartItemId)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);

            if (cartItem == null)
                return NotFound("Cart item not found");

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                _unitOfWork.CartItems.Update(cartItem);
                await _unitOfWork.Complete();
                return Ok(new { message = "Quantity decreased" });
            }
            else
            {
                _unitOfWork.CartItems.Remove(cartItem);
                await _unitOfWork.Complete();
                return Ok(new { message = "Item removed from cart" });
            }
        }


        [HttpPost("move-to-wishlist/{cartItemId}")]
        public async Task<IActionResult> MoveToWishlist(int cartItemId)
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (cartItem == null)
                return NotFound("Cart item not found");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var wishlist = await _unitOfWork.Wishlist.FindSingle(w => w.UserId == userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId };
                await _unitOfWork.Wishlist.AddAsync(wishlist);
            }

            if (cartItem.ProductVariant?.ProductId == null)
                return BadRequest("Invalid product data in cart item");

            var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductVariant.ProductId.Value);
            if (product == null)
                return NotFound("Product not found");

            if (!wishlist.Products.Contains(product))
            {
                wishlist.Products.Add(product);
            }

            _unitOfWork.CartItems.Remove(cartItem);
            await _unitOfWork.Complete();

            return Ok(new Response { Status = "success", Message = "Item moved to wishlist" });
        }


        //private string GetImgUrl(CartItem product)
        //{

        //    var imgs = product.ProductVariant.ProductImage.ToList();
        //    string imgUrl = imgs[0].ImageUrl;
        //    return imgUrl;
        //}

        [HttpGet("count")]
        public async Task<IActionResult> GetItemCount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            var totalItemsQuantity = user.Cart?.CartItems?.Sum(item => item.Quantity) ?? 0;

            return Ok(totalItemsQuantity);
        }

    }
}
