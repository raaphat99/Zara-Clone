using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("{id:int}/subcategories")]
        public async Task<IActionResult> GetSubCategoriesByParentId(int id)
        {
            var subCategories = await _unitOfWork.Categorys.GetSubCategoriesByParentIdAsync(id);

            if (subCategories == null || !subCategories.Any())
            {
                return NotFound($"No subcategories found for the parent category with ID {id}.");
            }

            return Ok(subCategories);
        }
    }
}
