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
using Microsoft.EntityFrameworkCore;

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

        [Authorize(Roles=UserRoles.Admin)]  
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _unitOfWork.Users.GetAll().ToList();
            var orders = _unitOfWork.Orders.GetAll().ToList();
            var addresses = _unitOfWork.UserAddress.GetAll().ToList();
            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var userAddress = addresses.FirstOrDefault(a => (a.UserId == user.Id) && (a.Active == true));
                // Check if userAddress exists
                if (userAddress == null) continue;  // Skip this user if no active address found

                var userOrders = orders.Where(o => o.UserId == user.Id);
                var orderDTOs = new List<OrderDTO>();

                foreach (var order in userOrders)
                {
                    var orderitemDtOs = new List<OrderItemDTO>();

                    // Null check for OrderItems collection
                    if (order.OrderItems != null)
                    {
                        foreach (var orderitem in order.OrderItems)
                        {
                            // Null checks for nested properties
                            if (orderitem?.ProductVariant?.Product == null) continue;

                            var orderitemDTO = new OrderItemDTO
                            {
                                name = orderitem.ProductVariant.Product.Name,
                                // Null check for ProductImage collection
                                productImage = orderitem.ProductVariant.ProductImage?.FirstOrDefault()?.ImageUrl,
                                color = orderitem.ProductVariant.ProductColor.ToString(),
                                quantity = orderitem.Quantity,
                                size = orderitem.ProductVariant.Size?.Value.ToString(),
                                subtotal = orderitem.Subtotal,
                                unitPrice = orderitem.UnitPrice,
                            };
                            orderitemDtOs.Add(orderitemDTO);
                        }
                    }

                    var orderDTO = new OrderDTO
                    {
                        id = order.Id,
                        customerName = $"{user.Name} {user.Surname}".Trim(),
                        created = order.Created.ToString(),
                        items = orderitemDtOs,
                        status = order.Status.ToString(),
                        totalPrice = order.TotalPrice,
                        trackingNumber = order.TrackingNumber
                    };
                    orderDTOs.Add(orderDTO);
                }

                var userAddressDTO = new UserAddressDTO
                {
                    Active = true,
                    City = userAddress.City,
                    Country = "Egypt",
                    Id = userAddress.Id,
                    State = userAddress.State,
                    Street = userAddress.Street,
                    UserId = userAddress.UserId
                };

                // Null check for UserMeasurements
                var activeMeasurement = user.UserMeasurements?.FirstOrDefault(m => m.Active == true);

                var userDTO = new UserDTO
                {
                    ActiveAddress = userAddressDTO,
                    Id = user.Id,
                    ActiveMesurment = activeMeasurement?.SizeValue,
                    Email = user.Email,
                    Name = $"{user.Name} {user.Surname}".Trim(),
                    Orders = orderDTOs,
                    PhoneNumber = userAddress.PhoneNumber,
                };
                userDTOs.Add(userDTO);
            }

            if (userDTOs.Count > 0)
                return Ok(userDTOs);
            return BadRequest("fe haga ghalat");
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
                    expiration = token.ValidTo,
                    role = userRoles
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
                UserName = model.Email

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = $"User creation failed! Errors: {errors}"
                });
                //return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            }
            await _userManager.AddToRoleAsync(user, UserRoles.User);

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        [Authorize(Roles=UserRoles.Admin)]
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
                UserName = model.Email
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
            var userId =  User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            var user = await _userManager.FindByIdAsync(userId);  // Await the task to get the user object

            if (user == null)
            {
                return NotFound();  // Handle case where the user is not found
            }
            var notifications = _unitOfWork.Notifications.GetAll().Where(n => n.UserId == user.Id);
            if (notifications != null)
            {
                _unitOfWork.Notifications.RemoveRange(notifications);
                await _unitOfWork.Complete();

            }
            var cart = await _unitOfWork.Carts.FindSingle(c => c.UserId == userId);
            if (cart != null)
            {
                _unitOfWork.Carts.Remove(cart);
                await _unitOfWork.Complete();

            }

            var wishlist = await _unitOfWork.Wishlist.FindSingle(w => w.UserId == userId);
            if (wishlist != null)
            {
                _unitOfWork.Wishlist.Remove(wishlist);
                await _unitOfWork.Complete();

            }

            var roles = await _userManager.GetRolesAsync(user);

            // Remove user from roles
            if (roles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
            }

            // Save changes to the database
            //await _unitOfWork.Complete();
            //_unitOfWork.Users.Remove(user);
            var result = await _userManager.DeleteAsync(user);  // Now user is of type User

            //await _unitOfWork.Complete();
            return Ok(new Response { Status = $"Success {result}", Message = "Account deleted successfully" });
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
