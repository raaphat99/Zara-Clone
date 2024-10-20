using Domain.Enums;
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
                productId = productVariant.ProductId ?? 0,
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
        public async Task<IActionResult> AddProductVariant(PVDTO productVariantDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(new { Errors = errors });
            }

            if (!Enum.TryParse(typeof(Color), productVariantDto.ProductColor, true, out var colorEnum))
            {
                return BadRequest($"Invalid color: {productVariantDto.ProductColor}. Please use a valid color.");
            }

            if (!Enum.TryParse(typeof(Material), productVariantDto.ProductMaterial, true, out var materialEnum))
            {
                return BadRequest($"Invalid material: {productVariantDto.ProductMaterial}. Please use a valid material.");
            }

            // تحقق من وجود متغيرات مشابهة بنفس الخصائص
            var existingVariant = await _unitOfWork.ProductVariant.FindAsync(v =>
                v.ProductId == productVariantDto.ProductId &&
                v.SizeId == productVariantDto.SizeId &&
                v.ProductColor == (Color)colorEnum && // تحويل إلى ProductColor
                v.ProductMaterial == (Material)materialEnum); // تحويل إلى ProductMaterial

            if (existingVariant.Any()) // استخدم Any() لسهولة القراءة
            {
                return Conflict("A product variant with the same color, material, size, and product ID already exists.");
            }

            // إنشاء متغير المنتج الجديد
            var productVariant = new ProductVariant
            {
                ProductId = productVariantDto.ProductId,
                SizeId = productVariantDto.SizeId,
                Price = productVariantDto.Price,
                StockQuantity = productVariantDto.StockQuantity,
                ProductColor = (Color)colorEnum, // تخزين قيمة enum
                ProductMaterial = (Material)materialEnum, // تخزين قيمة enum
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            // إضافة المتغير الجديد إلى قاعدة البيانات
            await _unitOfWork.ProductVariant.AddAsync(productVariant);
            await _unitOfWork.Complete();

            // إرجاع المتغير المنتج الذي تم إنشاؤه
            return CreatedAtAction(nameof(GetProductVariantById), new { id = productVariant.Id }, productVariant);
        }




        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProductVariant(int id, [FromBody] PVDTO productVariantDto)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(id);

            if (productVariant == null)
            {
                return NotFound($"Product Variant with ID {id} not found.");
            }

            if (Enum.TryParse(typeof(Color), productVariantDto.ProductColor, out var colorEnum))
            {
                productVariant.ProductColor = (Color)colorEnum;
            }
            else
            {
                return BadRequest($"Invalid Product Color value: {productVariantDto.ProductColor}");
            }

            if (Enum.TryParse(typeof(Material), productVariantDto.ProductMaterial, out var materialEnum))
            {
                productVariant.ProductMaterial = (Material)materialEnum;
            }
            else
            {
                return BadRequest($"Invalid Product Material value: {productVariantDto.ProductMaterial}");
            }

            productVariant.Price = productVariantDto.Price;
            productVariant.StockQuantity = productVariantDto.StockQuantity;
            productVariant.SizeId = productVariantDto.SizeId;
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
            var x = await _unitOfWork.ProductImages.GetImagesByVariantIdAsync(id);
            if(x != null)
            {
                await _unitOfWork.ProductImages.DeleteImagesByVariantIdAsync(id);
            }
            _unitOfWork.ProductVariant.Remove(productVariant);
            await _unitOfWork.Complete();

            return Ok(new { message = "Product successfully deleted.", deletedProductId = id });
            //return NoContent();
        }
    }
}
