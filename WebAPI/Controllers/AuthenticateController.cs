using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Domain.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly static int Tnumber;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public AuthenticateController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _unitOfWork.Users.GetAll();
            List<string> Ids = new List<string>();
            foreach (var item in users)
            {
                Ids.Add(item.Id);
            }
            return Ok(Ids);

        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sid,user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Name,($"{user.Name} {user.Surname}")),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Surname = model.Surname,
                UserName = (model.Name + model.Surname)

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            }
            await _userManager.AddToRoleAsync(user, UserRoles.User);

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost("register-admin")]

        public async Task<IActionResult> RegisterAdmin(RegisterModel model)
        {

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Surname = model.Surname,
                UserName = (model.Name + model.Surname)
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "Admin created successfully!" });
        }


        [HttpPut]
        public async Task<IActionResult> UpdateEmail(UpdateEmailDTO request)
        {

            // Check if the request is valid
            if (request == null || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.NewEmail))
            {
                return BadRequest(new Response { Status = "Error", Message = "Invalid request data" });
            }

            // Extract the email from the JWT token
            string userEmail = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token or email not found in token" });
            }

            // Retrieve the user by email
            var user = await _userManager.FindByIdAsync(userEmail);
            if (user == null)
            {
                return NotFound(new Response { Status = "Error", Message = "User not found" });
            }

            // Check if the provided password is valid
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return BadRequest(new Response { Status = "Error", Message = "Invalid password" });
            }

            // Ensure the new email isn't already taken by another user
            var existingUser = await _userManager.FindByEmailAsync(request.NewEmail);
            if (existingUser != null)
            {
                return BadRequest(new Response { Status = "Error", Message = "Email already in use" });
            }

            // Update the email
            user.Email = request.NewEmail;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(500, new Response { Status = "Error", Message = "Failed to update email" });
            }

            return Ok(new Response { Status = "Success", Message = "Email updated successfully" });

        }
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO request)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            if (userId == null)
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token or email not found in token" });

            var user = await _unitOfWork.Users.FindSingle(u => u.Id == userId);
            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new Response { Status = "Error", Message = "Invalid password" });

            return Ok(new Response { Status = "Success", Message = "Password changed successfully" });

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _unitOfWork.Users.FindSingle(u => u.Id == User.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (user == null)
                return NotFound();
            _unitOfWork.Users.Remove(user);
            _unitOfWork.Complete();
            return Ok(new Response { Status = "Success", Message = "Account deleted successfully" });
        }



        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }

}
