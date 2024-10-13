using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ProductVariantController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public ProductVariantController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("api/products/{productId}/variants")]
        public async Task<ActionResult<IEnumerable<VariantForDetailsScreenDto>>> GetVariantsByProductId(int productId)
        {
            var variants = await _unitOfWork.Products.GetAllVariants()
                .Where(v => v.ProductId == productId)
                .Select(v => new VariantForDetailsScreenDto
                {
                    Id = v.Id,
                    Price = v.Price,
                    DiscountPercentage = v.DiscountPercentage,
                    DiscountedPrice = v.Price * (1 - (v.DiscountPercentage / 100)),
                    StockQuantity = v.StockQuntity,
                    Created = v.Created,
                    Updated = v.Updated,
                    ProductColor = v.ProductColor.ToString(),  // Assuming it's an enum or string
                    ProductMaterial = v.ProductMaterial.ToString(),  // Assuming it's an enum or string
                    SizeId = v.SizeId,
                    SizeName = v.Size.Value.ToString(),  // Selecting only necessary property from related entity
                    ImageUrls = v.ProductImage.Select(i => i.ImageUrl).ToList()  // Selecting only image URLs
                })
                .ToListAsync();

            if (variants == null || !variants.Any())
            {
                return NotFound();
            }

            return Ok(variants);
        }

    }
}
