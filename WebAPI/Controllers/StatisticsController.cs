using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public StatisticsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("orders/count")]
        public async Task<IActionResult> GetOrdersCount()
        {
            var orderCount = await _unitOfWork.Orders.CountAsync();
            return Ok(orderCount);
        }

        [HttpGet("products/count")]
        public async Task<IActionResult> GetProductsCount()
        {
            var productCount = await _unitOfWork.Products.CountAsync();
            return Ok(productCount);
        }

        [HttpGet("categories/count")]
        public async Task<IActionResult> GetCategoriesCount()
        {
            var categoryCount = await _unitOfWork.Categories.CountAsync();
            return Ok(categoryCount);
        }

        [HttpGet("users/count")]
        public async Task<IActionResult> GetUsersCount()
        {
            var userCount = await _unitOfWork.Users.CountAsync();
            return Ok(userCount);
        }
    }
}

