using HotelListing.API.Data;
using HotelListing.API.InterFace;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace HotelListing.API.handlers;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IOptionsMonitor<AuthenticationSchemeOptions> _options;
    private ILoggerFactory _logger;
    private UrlEncoder _encoder;
    private IUserService _userService;
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IUserService userService) : base(options, logger, encoder)
    {
        _options = options;
        _logger = logger;
        _encoder = encoder;
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            return AuthenticateResult.Fail("Authorization header missing.");
        }

        var authHeader = authorizationHeader.ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Invalid authorization scheme.");
        }
        var token = authHeader.Substring("Basic ".Length).Trim();
        string decodedCredentials;
        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.Fail("Invalid authorization token.");
        }
        try
        {
            var credentialBytes = Convert.FromBase64String(token);
            decodedCredentials = System.Text.Encoding.UTF8.GetString(credentialBytes);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid authorization token.");
        }
        var credentials = decodedCredentials.Split(':', 2);
        if (credentials.Length != 2)
        {
            return AuthenticateResult.Fail("Invalid authorization token format.");
        }
        var email = credentials[0];
        var password = credentials[1];

        var loginDto = new DTOs.Auth.LoginUserDto
        {
            Email = email,
            Password = password
        };
        var user = await _userService.GetUserByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return AuthenticateResult.Fail("Invalid email or password.");
        }
        var isPasswordValid = await _userService.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            return AuthenticateResult.Fail("Invalid email or password.");
        }
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, email),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
