using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserMesurmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserMesurmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllMesurments()
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return NotFound("User not found!");

            var mesurments = user.UserMeasurements.ToList();

            if (!mesurments.Any())
                return NotFound("No mesurments found fot this user!");

            List<UserMesurmentDTO> userMesurmentDTOs = new List<UserMesurmentDTO>();

            foreach (var mesurment in mesurments)
            {
                var mesurmentDTO = new UserMesurmentDTO()
                {
                    Id = mesurment.Id,
                    MesurmentProfileName = mesurment.MesurmentProfileName,
                    Age = mesurment.Age,
                    Height = mesurment.Height,
                    Weight = mesurment.Weight,
                    FavoriteSection = mesurment.FavoriteSection,
                    SizeValue = mesurment.SizeValue,
                    Active = mesurment.Active,
                    Created = mesurment.Created,
                    Updated = mesurment.Updated
                };
                userMesurmentDTOs.Add(mesurmentDTO);
            }
            return Ok(userMesurmentDTOs);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserMeasurement>> GetMeasurementById(int id)
        {
            var mesurment = await _unitOfWork.UserMeasurements.GetByIdAsync(id);

            if (mesurment == null)
                return NotFound();

            UserMesurmentDTO mesurmentDTO = new UserMesurmentDTO()
            {
                Id = mesurment.Id,
                MesurmentProfileName = mesurment.MesurmentProfileName,
                FavoriteSection = mesurment.FavoriteSection,
                SizeValue = mesurment.SizeValue,
                Height = mesurment.Height,
                Weight = mesurment.Weight,
                Age = mesurment.Age,
                Active = mesurment.Active
            };

            return Ok(mesurmentDTO);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMeasurement([FromBody] UserMesurmentDTO measurementDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Extract the userId from the JWT token
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID could not be found in the token.");

            // Calculate size based on weight and height
            string size = CalculateSize(measurementDTO.Weight, measurementDTO.Height);

            UserMeasurement measurement = new UserMeasurement
            {
                MesurmentProfileName = measurementDTO.MesurmentProfileName,
                Age = measurementDTO.Age,
                Height = measurementDTO.Height,
                Weight = measurementDTO.Weight,
                FavoriteSection = measurementDTO.FavoriteSection,
                SizeValue = size,
                Active = measurementDTO.Active,
                Created = DateTime.Now
            };

            // Get the user by the userId extracted from the token
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Add the measurement to the user's profile
            var userMeasurements = user.UserMeasurements.ToList();
            // Set the new measurement as active if there are no previous measurements
            if (!userMeasurements.Any())
                measurement.Active = true;

            user.UserMeasurements.Add(measurement);
            // Save changes to the database
            await _unitOfWork.Complete();

            // Prepare the DTO to return
            var measurementDto = new UserMesurmentDTO
            {
                MesurmentProfileName = measurement.MesurmentProfileName,
                Age = measurement.Age,
                Height = measurement.Height,
                Weight = measurement.Weight,
                FavoriteSection = measurement.FavoriteSection,
                SizeValue = measurement.SizeValue,
                Created = measurement.Created
            };

            return CreatedAtAction(nameof(GetMeasurementById), new { id = measurement.Id }, measurementDto);
        }


        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditMesurment(UserMesurmentDTO mesurmentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            UserMeasurement mesurment = (UserMeasurement)_unitOfWork.UserMeasurements.Find(m => m.Id == mesurmentDTO.Id);
            mesurment = UpdateFromDto(mesurment, mesurmentDTO); //fill mesurment with new data from DTO
            _unitOfWork.UserMeasurements.Update(mesurment);
            await _unitOfWork.Complete();
            return Ok(mesurment);
        }


        [HttpDelete("{MesurmentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMesurment(int MesurmentId)
        {
            var mesurment = await _unitOfWork.UserMeasurements.GetByIdAsync(MesurmentId);
            if (mesurment == null)
                return BadRequest();
            _unitOfWork.UserMeasurements.Remove(mesurment);
            await _unitOfWork.Complete();
            return Ok("User measurement is deleted successfully.");
        }


        private string CalculateSize(int weight, int height)
        {
            int heightIndex = 0;
            int weightIndex = 0;
            int finalIndex = 0;

            string size;
            #region height index
            if (height >= 160 && height <= 170)
            {
                heightIndex = 1;
            }
            else if (height > 170 && height <= 180)
            {
                heightIndex = 2;
            }
            else if (height > 180 && height <= 190)
            {
                heightIndex = 3;
            }
            else if (height > 190 && height <= 200)
            {
                heightIndex = 4;
            }
            #endregion
            #region weight Index
            if (weight >= 40 && weight <= 60)
            {
                weightIndex = 1;
            }
            else if (weight > 60 && weight <= 80)
            {
                weightIndex = 2;
            }
            else if (weight > 80 && weight <= 100)
            {
                weightIndex = 3;
            }
            else if (weight > 100 && weight <= 120)
            {
                weightIndex = 4;
            }
            #endregion
            finalIndex = heightIndex > weightIndex ? heightIndex : weightIndex;
            switch (finalIndex)
            {
                case 1:
                    return "S";
                case 2:
                    return "M";
                case 3:
                    return "L";
                case 4:
                    return "XL";
                default:
                    throw new ArgumentOutOfRangeException("Invalid finalIndex value");
            }


        }


        private UserMeasurement UpdateFromDto(UserMeasurement mesurment, UserMesurmentDTO mesurmentDTO)
        {
            string size = CalculateSize(mesurmentDTO.Weight, mesurmentDTO.Height);

            mesurment.SizeValue = size;
            mesurment.Weight = mesurmentDTO.Weight;
            mesurment.Height = mesurmentDTO.Height;
            mesurment.Age = mesurmentDTO.Age;
            mesurment.MesurmentProfileName = mesurmentDTO.MesurmentProfileName;
            mesurment.FavoriteSection = mesurmentDTO.FavoriteSection;
            mesurment.Updated = DateTime.Now;
            return mesurment;
        }
    }
}
