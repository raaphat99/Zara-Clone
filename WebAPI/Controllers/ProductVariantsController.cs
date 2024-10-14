using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.ProductDTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductVariantsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductVariantById(int id)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(id);

            if (productVariant == null)
            {
                return NotFound($"Product variant with ID {id} not found.");
            }
            var productvariantdto = new ProductVariantforC_M_P()
            {
                Id = productVariant.Id,
                ProductId = productVariant.ProductId ?? 0,
                ProductName = productVariant.Product.Name,
                Price = productVariant.Price,
                ProductColor = productVariant.ProductColor,
                // SizeName = productVariant.Product.,
                // SizeId = productVariant.SizeId,
                StockQuantity = productVariant.StockQuantity,
                ProductMaterial = productVariant.ProductMaterial,
                Created = productVariant.Created,
                Updated = productVariant.Updated
            };
            return Ok(productvariantdto);
        }


        [HttpPost]
        public async Task<IActionResult> AddProductVariant([FromBody] ProductVariantforC_M_P productVariantDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productVariant = new ProductVariant
            {
                ProductId = productVariantDto.ProductId,
                SizeId = productVariantDto.SizeId,
                Price = productVariantDto.Price,
                StockQuantity = productVariantDto.StockQuantity,
                ProductColor = productVariantDto.ProductColor,
                ProductMaterial = productVariantDto.ProductMaterial,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            await _unitOfWork.ProductVariant.AddAsync(productVariant);
            await _unitOfWork.Complete();

            return CreatedAtAction(nameof(GetProductVariantById), new { id = productVariant.Id }, productVariant);
        }


        [HttpPut("{id:int}")] //  مش جربتها 
        public async Task<IActionResult> UpdateProductVariant(int id, [FromBody] ProductVariantforC_M_P productVariantDto)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(id);

            if (productVariant == null)
            {
                return NotFound($"Product Variant with ID {id} not found.");
            }

            productVariant.Price = productVariantDto.Price;
            productVariant.StockQuantity = productVariantDto.StockQuantity;
            productVariant.ProductColor = productVariantDto.ProductColor;
            productVariant.ProductMaterial = productVariantDto.ProductMaterial;
            productVariant.Updated = DateTime.UtcNow;

            _unitOfWork.ProductVariant.Update(productVariant);
            await _unitOfWork.Complete();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProductVariant(int id)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(id);

            if (productVariant == null)
            {
                return NotFound($"No product found with ID {id}.");
            }

            _unitOfWork.ProductVariant.Remove(productVariant);
            await _unitOfWork.Complete();

            return Ok(new { message = "Product successfully deleted.", deletedProductId = id });
            //return NoContent();
        }
    }
}
