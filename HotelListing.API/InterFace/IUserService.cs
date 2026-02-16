using HotelListing.API.Data;
using HotelListing.API.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.InterFace
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<string> GenerateToken(ApplicationUser user);
        Task<IdentityResult> AddUserToRoleAsync(RegisterUserDto registerUserDto, string role);

    }
}
