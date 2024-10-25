using Amazon.S3;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using WebAPI.DTOs.ProductDTOs;
using WebAPI.Services;
using static Amazon.S3.Util.S3EventNotification;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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
        [HttpGet("variants/{variantId:int}")]
        [HttpGet("{variantId}")]
        public async Task<ActionResult<IEnumerable<ProductImageDTO>>> GetImagesByVariant(int variantId)
        {
            var images = await _unitOfWork.ProductImages.GetImagesByVariantIdAsync(variantId);

            if (images == null || !images.Any())
            {
                return NotFound($"No images found for Product Variant ID {variantId}.");
            }

            var pimage = images.Select(image => new ProductImageDTO
            {
                Id = image.Id,
                AlternativeText = image.AlternativeText,
                ImageUrl = image.ImageUrl,
            }).ToList();

            return Ok(pimage);
        }



        [HttpPost("/{variantId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductImage>> UploadImage(IFormFile file, int variantId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Get the file name
            var fileName = Path.GetFileName(file.FileName);

            try
            {
                using (var stream = new MemoryStream())
                {
                    // Copy the file to the stream and upload to S3
                    await file.CopyToAsync(stream);
                    await _s3Service.UploadFileAsync(file);
                }

                // Generate the public URL (replace with your actual S3 bucket name)
                var publicUrl = $"https://{bucketName}.s3.amazonaws.com/{fileName}";

                // Create a new ProductImage instance
                ProductImage image = new ProductImage()
                {
                    ProductVariantId = variantId,
                    ImageUrl = publicUrl,
                };

                // Add the image to the database and save changes
                await _unitOfWork.ProductImages.AddAsync(image);
                await _unitOfWork.Complete();

                // Return the public URL of the uploaded image
                return Ok(publicUrl);
            }
            catch (AmazonS3Exception s3Ex)
            {
                // Handle S3-specific errors
                return StatusCode(500, $"S3 Error: {s3Ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle general errors
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
