using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;

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
            var subCategories = await _unitOfWork.Categories.GetSubCategoriesByParentIdAsync(id);

            if (subCategories == null || !subCategories.Any())
            {
                return NotFound($"No subcategories found for the parent category with ID {id}.");
            }
            List<CategoryDto> categoryDtos = new List<CategoryDto>();
            foreach (var category in subCategories)
            {
                categoryDtos.Add(new CategoryDto()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ParentCategoryId = category.ParentCategoryId,
                    SizeTypeId = category.SizeTypeId,
                });
            }
            return Ok(categoryDtos);
        }



        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            var categorydto = new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                SizeTypeId = category.SizeTypeId,
            };

            return Ok(categorydto);
        }



        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto categorydto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = new Category()
            {
                Name = categorydto.Name,
                Description = categorydto.Description
            };
            if (categorydto.ParentCategoryId.HasValue)
            {
                category.ParentCategoryId = categorydto.ParentCategoryId;
            }
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.Complete();
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }



        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto updatedCategory)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            category.Name = updatedCategory.Name;
            category.Description = updatedCategory.Description;
            category.SizeTypeId = updatedCategory.SizeTypeId;
            category.ParentCategoryId = updatedCategory.ParentCategoryId;

            if (updatedCategory.ParentCategoryId.HasValue)
            {
                category.ParentCategoryId = updatedCategory.ParentCategoryId;
            }
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.Complete();

            return NoContent();
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.Complete();

            return NoContent();
        }



        [HttpGet("main-categories")]
        public async Task<IActionResult> GetMainCategories()
        {
            var mainCategories = await _unitOfWork.Categories.FindAsync(c => c.ParentCategoryId == null);
            List<CategoryDto> categorydto = new List<CategoryDto>();


            if (mainCategories == null || !mainCategories.Any())
            {
                return NotFound("No main categories found.");
            }
            foreach (var category in mainCategories)
            {
                var x = new CategoryDto()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    SizeTypeId = category.SizeTypeId,
                    ParentCategoryId = category.ParentCategoryId,

                };
                categorydto.Add(x);
            }
            return Ok(categorydto);
        }
    }
}
