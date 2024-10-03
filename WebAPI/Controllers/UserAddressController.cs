using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserAddressController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserAddresses(string userId)
        {
            var addresses = await _unitOfWork.UserAddress.GetAllByUserIdAsync(userId);
            var addressDTOs = addresses.Select(a => new UserAddressDTO
            {
                Id = a.Id,
                Country = a.Country,
                State = a.State,
                City = a.City,
                Street = a.Street,
                PostalCode = a.PostalCode,
                Active = a.Active.Value,
                UserId = a.UserId
            }).ToList();

            return Ok(addressDTOs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAddress([FromBody] UserAddressDTO addressDTO)
        {
            if (addressDTO == null)
            {
                return BadRequest();
            }

            // Check if this is the user's first address
            var userAddresses = await _unitOfWork.UserAddress.GetAllByUserIdAsync(addressDTO.UserId);

            // Create new UserAddress with Active set based on whether it's the first address
            var address = new UserAddress
            {
                Country = addressDTO.Country,
                State = addressDTO.State,
                City = addressDTO.City,
                Street = addressDTO.Street,
                PostalCode = addressDTO.PostalCode,
                UserId = addressDTO.UserId,
                Active = userAddresses.Count() == 0 // Set Active to true if first address, otherwise false
            };

            await _unitOfWork.UserAddress.AddAsync(address);
            await _unitOfWork.Complete();

            return CreatedAtAction(nameof(GetUserAddresses), new { userId = address.UserId }, address);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAddress(int id, [FromBody] UserAddressDTO addressDTO)
        {
            if (addressDTO == null || addressDTO.Id != id)
            {
                return BadRequest();
            }

            var address = await _unitOfWork.UserAddress.GetByIdAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            address.Country = addressDTO.Country;
            address.State = addressDTO.State;
            address.City = addressDTO.City;
            address.Street = addressDTO.Street;
            address.PostalCode = addressDTO.PostalCode;
            address.UserId = addressDTO.UserId; 

            _unitOfWork.UserAddress.Update(address);
            await _unitOfWork.Complete();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAddress(int id)
        {
            var address = await _unitOfWork.UserAddress.GetByIdAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            // Check if the address is active
            if (address.Active == true)
            {
                return BadRequest("The first address cannot be deleted.");
            }

            _unitOfWork.UserAddress.Remove(address);
            await _unitOfWork.Complete();

            return NoContent();
        }

    }
}
