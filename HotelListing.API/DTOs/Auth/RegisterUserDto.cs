using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.DTOs.Auth;

public class RegisterUserDto : LoginUserDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}

public class LoginUserDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, PasswordPropertyText]
    public string Password { get; set; } = string.Empty;
}

public class RegisteredUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}