using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DiscountProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // Get discounted products by category
        [HttpGet("special-prices")]
        public async Task<IActionResult> GetDiscountedProducts(int categoryId, int pageNumber = 1, int pageSize = 10)
        {
            var products = await _unitOfWork.Products.GetAll()
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            var discountedProducts = products
                .SelectMany(p => p.ProductVariants)
                .Where(v => v.DiscountPercentage > 0)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new ProductListDTO
                {
                    ID = v.Product.Id,
                    Name = v.Product.Name,
                    Price = v.Price,
                    DiscountPercentage = v.DiscountPercentage,
                    DiscountedPrice = v.DiscountedPrice,
                    StockQuantity = v.StockQuntity,
                    ProductImage = v.ProductImage
                        .OrderBy(img => img.SortOrder)
                        .Select(img => new ProductImageDTO
                        {
                            Id = img.Id,
                            ImageUrl = img.ImageUrl,
                            AlternativeText = img.AlternativeText
                        }).FirstOrDefault()
                });

            return Ok(discountedProducts);
        }

        // Get Discount product details by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDiscountProductById(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productDetail = new ProductDetailDTO
            {
                ID = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPercentage = product.ProductVariants.FirstOrDefault()?.DiscountPercentage ?? 0,
                DiscountedPrice = product.ProductVariants.FirstOrDefault()?.DiscountedPrice ?? product.Price,
                ProductVariants = product.ProductVariants.Select(v => new ProductvariantDTO
                {
                    Id = v.Id,
                    Price = v.Price,
                    DiscountPercentage = v.DiscountPercentage,
                    DiscountedPrice = v.DiscountedPrice,
                    ProductImages = v.ProductImage
                        .Select(img => new ProductImageDTO
                        {
                            Id = img.Id,
                            ImageUrl = img.ImageUrl,
                            AlternativeText = img.AlternativeText
                        }).ToList(),
                    ProductColors = GetColorName((int)v.ProductColor),
                    //AvailableSizes = v.ProductSizes.Select(s => new ProductSizeDTO
                    //{
                    //    Id = s.Id,
                    //    Type = s.Type,
                    //    Value = s.Value
                    //}).ToList()


                }).ToList()
            };

            return Ok(productDetail);
        }
        private string GetColorName(int colorValue)
        {
            if (Enum.IsDefined(typeof(Color), colorValue))
            {
                return Enum.GetName(typeof(Color), colorValue);
            }
            return "Unknown";
        }
    }

}

