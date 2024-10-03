using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    //[Route("api/[controller]")]
    public partial class ProductController : ControllerBase
    {
        [HttpGet("/api/products/search")]
        public async Task<ActionResult<List<Product>>> SearchProducts(
            [FromQuery] string searchTerm,
            [FromQuery] string category = null,
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

            return Ok(products);
        }



    }
}
