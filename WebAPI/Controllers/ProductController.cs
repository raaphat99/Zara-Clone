using DataAccess.EFCore.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace WebAPI.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/api/products")]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Products.GetAll();
            return Ok(products);
        }

        [HttpGet("/api/products/{id: int}")]
        public async Task<IActionResult> GetProductByID(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("/api/products/category/{categoryId}")]
        public IActionResult GetProductsByCategory(int categoryID)
        {
            var products = _unitOfWork.Products.Find(prd => prd.CategoryID == categoryID);
            return Ok(products);
        }

        [HttpGet("/api/products/{id: int}/variants")]
        public async Task<IActionResult> GetProductVariants(int id)
        {
            var product = await _unitOfWork.Products
                .Find(prd => prd.ID == id)
                .Include(prd => prd.ProductVariants)
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product.ProductVariants);
        }

        [HttpGet("/api/products/search")]
        public async Task<ActionResult<List<Product>>> SearchProducts(
            [FromQuery] string searchTerm,
            [FromQuery] string category = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string color = null,
            [FromQuery] string brand = null)
        {
            var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm, category, minPrice, maxPrice, color, brand);

            if (products == null || !products.Any())
            {
                return NotFound("No products found matching the search criteria.");
            }

            return Ok(products);
        }

        [HttpGet("{mainCategoryId:int}/")]
        public async Task<IActionResult> GetProductsByCategory(int mainCategoryId, int? subCategoryId = null)
        {
            var mainCategory = await _unitOfWork.Categories.GetByIdAsync(mainCategoryId);

            if (mainCategory == null)
            {
                return NotFound($"No category found with ID {mainCategoryId}.");
            }

            IQueryable<Product> products;

            if (subCategoryId.HasValue)
            {
                var subCategory = await _unitOfWork.Categories.GetByIdAsync(subCategoryId.Value);

                if (subCategory == null || subCategory.ParentCategoryID != mainCategoryId)
                {
                    return NotFound($"No subcategory found with ID {subCategoryId} under main category ID {mainCategoryId}.");
                }

                products = _unitOfWork.Products.Find(p => p.CategoryID == subCategoryId);
            }
            else
            {
                products = _unitOfWork.Products.Find(p => p.CategoryID == mainCategoryId);
            }

            if (products == null || !products.Any())
            {
                return NotFound("No products found for the specified category.");
            }

            return Ok(products);
        }




    }
}
