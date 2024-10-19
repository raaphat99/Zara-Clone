using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserAddressController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            if (userId == null)
                return Unauthorized("User ID not found in token");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return NotFound("User not found!");
            var addresses = await _unitOfWork.UserAddress.GetAllByUserIdAsync(userId);
            var addressDTOs = addresses.Select(a => new UserAddressDTO
            {
                Id = a.Id,
                PhoneNumber = a.PhoneNumber,
                Country = a.Country,
                State = a.State,
                City = a.City,
                Street = a.Street,
                Active = a.Active.Value,
                Area = a.AddressMoreInfo
            });

            return Ok(addressDTOs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAddress([FromBody] UserAddressDTO addressDTO)
        {
            if (addressDTO == null)
            {
                return BadRequest();
            }

            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;

            // Check if this is the user's first address
            var userAddresses = await _unitOfWork.UserAddress.GetAllByUserIdAsync(userId);

            // Create new UserAddress with Active set based on whether it's the first address
            var address = new UserAddress
            {
                UserId = userId,
                AddressMoreInfo = addressDTO.Area,
                PhoneNumber = addressDTO.PhoneNumber,
                Country = addressDTO.Country,
                State = addressDTO.State,
                City = addressDTO.City,
                Street = addressDTO.Street,
                Active = userAddresses.Count() == 0 // Set Active to true if first address, otherwise false
            };

            await _unitOfWork.UserAddress.AddAsync(address);
            await _unitOfWork.Complete();

            //return CreatedAtAction(nameof(GetUserAddresses), new { userId = address.UserId }, address);
            return Ok(userAddresses.Count());
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserAddress([FromBody] UserAddressDTO addressDTO)
        {

            var address = await _unitOfWork.UserAddress.GetByIdAsync(addressDTO.Id);
            if (address == null)
            {
                return NotFound();
            }

            address.PhoneNumber = addressDTO.PhoneNumber;
            address.Country = addressDTO.Country;
            address.State = addressDTO.State;
            address.City = addressDTO.City;
            address.Street = addressDTO.Street;
            address.AddressMoreInfo = addressDTO.Area;
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