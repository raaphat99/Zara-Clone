using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetMostExpensiveProducts([FromQuery] int count)
        {
            var expensiveProducts = _unitOfWork.Products.GetMostExpensiveProducts(count);
            return Ok(expensiveProducts);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct()
        {
            var product = new Product
            {
                CategoryID = 101,
                Name = "Washing Machine",
                Description = "Lorem ipsum sit amit",
                Price = 7500.00,
                StockQuantity = 10,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.Complete();
            return Ok();

        }

    }
}
