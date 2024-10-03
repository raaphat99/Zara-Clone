using DataAccess.EFCore.Repositories;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public partial class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //    #region Expensive ProductDto
        //    var expensiveProducts = _unitOfWork.Products.GetMostExpensiveProducts(count);
        //    List<ProductDto> productdto = new List<ProductDto>();
        //        foreach (var product in expensiveProducts)
        //        {
        //            var x = new ProductDto()
        //            {
        //                Id = product.Id,
        //                Name = product.Name,
        //                Price = product.Price,
        //                StockQuntity = product.StockQuntity
        //            };
        //    productdto.Add(x);
        //        }
        //        return Ok(productdto);
        //#endregion

        [HttpGet("/api/products")]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Products.GetAll();
            return Ok(products);
        }



        [HttpGet("/api/products/{id:int}")]
        public async Task<IActionResult> GetProductByID(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }



        [HttpGet("/api/products/category/{categoryId:int}")]
        public IActionResult GetProductsByCategory(int categoryID)
        {
            var products = _unitOfWork.Products.Find(prd => prd.CategoryId == categoryID);
            return Ok(products);
        }



        [HttpGet("/api/products/{id:int}/variants")]
        public async Task<IActionResult> GetProductVariants(int id)
        {
            var product = await _unitOfWork.Products
                .Find(prd => prd.Id == id)
                .Include(prd => prd.ProductVariants)
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product.ProductVariants);
        }



        // Get all products with id=7 woman and all dress with id=10
        [HttpGet("/api/products/{mainCategoryId:int}")]
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

                if (subCategory == null || subCategory.ParentCategoryId != mainCategoryId)
                {
                    return NotFound($"No subcategory found with ID {subCategoryId} under main category ID {mainCategoryId}.");
                }

                products = _unitOfWork.Products.Find(p => p.CategoryId == subCategoryId);
            }
            else
            {
                products = _unitOfWork.Products.Find(p => p.CategoryId == mainCategoryId);
            }

            if (products == null || !products.Any())
            {
                return NotFound("No products found for the specified category.");
            }

            return Ok(products);
        }

    }
}
