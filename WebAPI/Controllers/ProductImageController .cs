using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using WebAPI.Services;
using static Amazon.S3.Util.S3EventNotification;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductImageController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly S3Service _s3Service;
        private readonly string bucketName;

        public ProductImageController(IUnitOfWork unitOfWork, S3Service s3Service, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _s3Service = s3Service;
            bucketName = configuration["AWS:BucketName"];
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


        [HttpPost("/{variantId:int}")]
        public async Task<ActionResult<ProductImage>> UploadImage(IFormFile file, int variantId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            //var fileExtension = Path.GetExtension(file.FileName);
            //var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var fileName = Path.GetFileName(file.FileName);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    await _s3Service.UploadFileAsync(file);
                }

                var publicUrl = $"https://{bucketName}.s3.amazonaws.com/{fileName}";

                ProductImage image = new ProductImage()
                {
                    ProductVariantId = variantId,
                    ImageUrl = publicUrl,
                };

                await _unitOfWork.ProductImages.AddAsync(image);
                await _unitOfWork.Complete();

                return Ok("Image added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading file: {ex.Message}");
            }
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
