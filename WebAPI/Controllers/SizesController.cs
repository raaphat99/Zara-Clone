﻿using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public SizesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> AddSize([FromBody] SizeDTO sizeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure the SizeTypeId (refers to one of three values "Alpha|Numeric|Age-based") is provided, otherwise return a bad request
            if (sizeDto.SizeType == null)
            {
                return BadRequest("SizeType is required.");
            }

            // Map the DTO to the Size entity
            var newSize = new Size
            {
                // The keyword out indicates that "sizeTypeId" will store the result of the TryParse operation if it succeeds. (true or false)
                SizeTypeId = int.TryParse(sizeDto.SizeType, out var sizeTypeId) ? sizeTypeId : null,
                Value = sizeDto.Value
            };

            // Add the new size to the repository and save changes
            await _unitOfWork.Sizes.AddAsync(newSize);
            await _unitOfWork.Complete();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSize(int id)
        {
            var size = await _unitOfWork.Sizes.GetByIdAsync(id);

            if (size == null)
            {
                return NotFound($"No Size with ID {id} found.");
            }

            _unitOfWork.Sizes.Remove(size);
            await _unitOfWork.Complete();

            return NoContent();
        }
    }
}