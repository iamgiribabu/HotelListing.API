using HotelListing.API.Data;
using HotelListing.API.DTOs.Auth;
using HotelListing.API.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            try
            {
                var existingUser = await _userService.GetUserByEmailAsync(registerUserDto.Email.Trim());
                if (existingUser != null)
                {
                    return BadRequest(new { message = "User with this email already exists" });
                }
                var result = await _userService.RegisterUserAsync(registerUserDto);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                var isUserRoleAddd = await _userService.AddUserToRoleAsync(registerUserDto, registerUserDto.Role);
                if (!isUserRoleAddd.Succeeded) {    
                    foreach (var error in isUserRoleAddd.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }   
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await _userService.GetUserByEmailAsync(loginUserDto.Email);
            if (user == null || !await _userService.CheckPasswordAsync(user, loginUserDto.Password))
            {
                return Unauthorized(loginUserDto);
            }
            var token = await _userService.GenerateToken(user);

            return Ok($"Welcome back {user.FullName} {token}!");
        }
    }
}
