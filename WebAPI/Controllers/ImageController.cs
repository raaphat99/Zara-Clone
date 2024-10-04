using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ImageController : ControllerBase
    {
        private readonly S3Service _s3Service;
        private readonly string bucketName;


        public ImageController(S3Service s3Service, IConfiguration configuration)
        {
            _s3Service = s3Service;
            bucketName = configuration["AWS:BucketName"];
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
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
                return Ok(new { Url = publicUrl });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading file: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("get-image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var imageUrl = $"https://{bucketName}.s3.amazonaws.com/{fileName}";
            return Ok(new { Url = imageUrl });
        }


        [HttpPut("update-image/{fileName}")]
        public async Task<IActionResult> UpdateImage(string fileName, IFormFile newFile)
        {
            try
            {
                await _s3Service.DeleteFileAsync(fileName, bucketName);
                await _s3Service.UploadFileAsync(newFile);
                return Ok(new { Message = "Image updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating file: {ex.Message}");
            }
        }


        [HttpDelete("delete-image/{fileName}")]
        public async Task<IActionResult> DeleteImage(string fileName)
        {
            try
            {
                await _s3Service.DeleteFileAsync(fileName, bucketName);
                return Ok(new { Message = "Image deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting file: {ex.Message}");
            }
        }


    }
}
