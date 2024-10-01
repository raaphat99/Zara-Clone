﻿using DataAccess.EFCore.Repositories;
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
    public class ProductController : ControllerBase
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
<<<<<<< HEAD
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
=======
                return NotFound("No products found matching the search criteria.");
            }
>>>>>>> raaphat

            return Ok(products);
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
