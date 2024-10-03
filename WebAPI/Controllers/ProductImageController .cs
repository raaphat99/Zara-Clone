using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductImageController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductImageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [AllowAnonymous]
        [HttpGet("{variantId}")]
        public async Task<ActionResult<IEnumerable<ProductImage>>> GetImagesByVariant(int variantId)
        {
            var images = await _unitOfWork.ProductImages.GetImagesByVariantIdAsync(variantId);
            if (images == null || !images.Any())
            {
                return NotFound();
            }
            return Ok(images);
        }


        [HttpPost]
        public async Task<ActionResult<ProductImage>> UploadImage([FromBody] ProductImage image)
        {
            await _unitOfWork.ProductImages.AddAsync(image);
            return CreatedAtAction(nameof(GetImagesByVariant), new { variantId = image.ProductVariantId }, image);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateImage(int id, [FromBody] ProductImage image)
        {
            if (id != image.Id)
            {
                return BadRequest();
            }
            _unitOfWork.ProductImages.Update(image);
            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _unitOfWork.ProductImages.GetByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            _unitOfWork.ProductImages.Remove(image);
            return Ok();
        }
    }
}
