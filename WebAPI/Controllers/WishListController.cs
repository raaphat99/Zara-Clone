using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public WishListController(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllItems( string userId) 
        {
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

        [HttpPost]
        public async Task<IActionResult> AddToWishList(int productId , string userId) 
        {
            Wishlist wishlist = (Wishlist) _unitOfWork.Wishlist.Find(w=>w.UserId == userId);
            var product = await _unitOfWork.Products.GetByIdAsync (productId);
            var products = wishlist.Products.ToList();
            if(products.Contains(product))
            {
                wishlist.Products.Remove(product);
                _unitOfWork.Wishlist.Update(wishlist);
                await _unitOfWork.Complete();
                return Ok("Product Removed");
            }
            else
            {
                wishlist.Products.Add(product);
                _unitOfWork.Wishlist.Update(wishlist);
                await _unitOfWork.Complete();
                return Ok("Product Added");

            }
        }


        private  string GetImgUrl(Product product) 
        {
            var variants = product.ProductVariants.ToList();
            var imgs = variants[0].ProductImage.ToList();
            string imgUrl = imgs[0].ImageUrl;
            return imgUrl;
        }
    }
}
