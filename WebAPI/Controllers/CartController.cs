using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddCartItem(int productVariantId, string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            List<CartItem> cartitems = user.Cart.CartItems.ToList();
            ProductVariant product = await _unitOfWork.ProductVariant.GetByIdAsync(productVariantId);
            if (product.StockQuantity == 0)
                return NotFound("Out Of Stock");
            bool exist = false;
            foreach (var item in cartitems)
            {
                if (item.ProductVariant.Id == product.Id)
                {
                    exist = true;
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
                return Ok("Item added");
            }
            else if (exist && product.StockQuantity > 0)
            {
                var item = await _unitOfWork.CartItems.GetByIdAsync(newitem.Id);
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
        public async Task<IActionResult> RemoveItemFromCart(int cartItemId)
        {
            var cartitem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            _unitOfWork.CartItems.Remove(cartitem);
            await _unitOfWork.Complete();
            return Ok("Item Deleted");
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
