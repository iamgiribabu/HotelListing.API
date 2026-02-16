using HotelListing.API.Data;
using HotelListing.API.DTOs.Auth;
using HotelListing.API.InterFace;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    public UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration ?? throw new Exception();
    }
    public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var normalizedEmail = registerUserDto.Email.Trim().ToLower();

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
        };
        return await _userManager.CreateAsync(user, registerUserDto.Password);
    }
    public async Task<IdentityResult> AddUserToRoleAsync(RegisterUserDto registerUserDto, string role)
    {
        var normalizedEmail = registerUserDto.Email.Trim().ToLower();

        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }
        return await _userManager.AddToRoleAsync(user, role);
    }
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();
        return await _userManager.FindByEmailAsync(normalizedEmail);
    }
    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }
    public async Task<string> GenerateToken(ApplicationUser user)
    {
        // set basic user claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName)

        };

        //set user roles claims
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        claims.AddRange(roleClaims);

        //set jwt key credentials
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //create an encoded token
        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentials
        );
        // return the token
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
