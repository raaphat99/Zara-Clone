using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.ProductDTOs;

namespace WebAPI.Controllers
{
    public partial class ProductsController : ControllerBase
    {
        [HttpGet("search")]
        public async Task<ActionResult<List<ProductDto>>> SearchProducts(
            [FromQuery] string searchTerm,
            [FromQuery] string? category = null,
            [FromQuery] double? minPrice = null,
            [FromQuery] double? maxPrice = null,
            [FromQuery] string? size = null,
            [FromQuery] Color? color = null,
            [FromQuery] Material? material = null)
        {
            var products = await _unitOfWork.Products.SearchProductsAsync(
                searchTerm,
                category,
                minPrice,
                maxPrice,
                size,
                color,
                material);

            if (products == null || products.Count == 0)
            {
                return NotFound("No products found matching the search criteria.");
            }

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Created = p.Created,
                Updated = p.Updated,
                CategoryId = p.CategoryId,
                MainImageUrl = p.ProductVariants.SelectMany(pv => pv.ProductImage).FirstOrDefault().ImageUrl
            }).ToList();

            return Ok(productDtos);
        }




    }
}
