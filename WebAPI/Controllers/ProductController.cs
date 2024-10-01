using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;

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
             List<ProductDto> productdto= new List<ProductDto>();
            foreach (var product in expensiveProducts)
            {
                var x=new ProductDto()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    StockQuntity = product.StockQuntity
                };
                productdto.Add(x);
            }
            return Ok(productdto);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct()
        {
            var product = new Product
            {
                CategoryId = 101,
                Name = "Washing Machine",
                Description = "Lorem ipsum sit amit",
                Price = 7500.00,
                StockQuntity = 10,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.Complete();
            return Ok();

        }
        //بتجيب كل product الي واخده id=7 woman and all dress with id=10
        [HttpGet("{mainCategoryId:int}/")]
        public async Task<IActionResult> GetProductsByCategory(int mainCategoryId, int? subCategoryId = null)
        {
            var mainCategory = await _unitOfWork.Categorys.GetByIdAsync(mainCategoryId);

            if (mainCategory == null)
            {
                return NotFound($"No category found with ID {mainCategoryId}.");
            }

            IEnumerable<Product> products;

            if (subCategoryId.HasValue)
            {
                var subCategory = await _unitOfWork.Categorys.GetByIdAsync(subCategoryId.Value);

                if (subCategory == null || subCategory.ParentCategoryId != mainCategoryId)
                {
                    return NotFound($"No subcategory found with ID {subCategoryId} under main category ID {mainCategoryId}.");
                }

                products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == subCategoryId);
            }
            else
            {
                products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == mainCategoryId);
            }

            if (products == null || !products.Any())
            {
                return NotFound("No products found for the specified category.");
            }

            return Ok(products);
        }




    }
}
