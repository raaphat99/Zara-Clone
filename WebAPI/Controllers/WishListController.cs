using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public WishListController(IUnitOfWork unitOfWork , UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpPost("add")]
        public async Task<ActionResult<Wishlist>> AddToWishlist([FromBody] AddToWishListDTO addToWishlistDto)
        {
            // Get the currently authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User is not logged in.");
            }

            // Create the wishlist item based on DTO input
            var wishlist = new Wishlist
            {
                UserId = user.Id,
                Products = new List<Product>
            {
                new Product { Id = addToWishlistDto.ProductId }
            }
            };

            var addedItem = await _unitOfWork.Wishlist.AddToWishList(wishlist);
            await _unitOfWork.Complete();

            return Ok(addedItem);
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveWishlistItem([FromQuery] int productId)
        {
            // Get the currently authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var wishlist = new Wishlist
            {
                UserId = user.Id,
                Products = new List<Product>
            {
                new Product { Id = productId }
            }
            };

            // Remove from wishlist via the Unit of Work
            await _unitOfWork.Wishlist.RemoveFromWishList(wishlist);
            await _unitOfWork.Complete();

            return Ok("Item removed from wishlist.");
        }

        [HttpGet("checkwish/{itemId}")]
        public async Task<IActionResult> CheckWishlist(int itemId)
        {
            // Get the currently authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User is not logged in.");
            }

            // Check if the item is in the user's wishlist
            bool isInWishlist = _unitOfWork.Wishlist.IsWishList(itemId, user.Id);

            return Ok(isInWishlist);
        }



    }
}
