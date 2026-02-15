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

    }
}
