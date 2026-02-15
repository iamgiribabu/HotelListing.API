using HotelListing.API.Data;
using HotelListing.API.InterFace;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace HotelListing.API.handlers;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IOptionsMonitor<AuthenticationSchemeOptions> _options;
    private ILoggerFactory _logger;
    private UrlEncoder _encoder;
    private IApiKeyValidatorService _apiKeyValidatorService;
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyValidatorService apiKeyValidatorService) : base(options, logger, encoder)
    {
        _options = options;
        _logger = logger;
        _encoder = encoder;
        _apiKeyValidatorService = apiKeyValidatorService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string apiKey = string.Empty;
        if (Request.Headers.TryGetValue(Constants.AuthenticationDefaults.ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            apiKey = apiKeyHeaderValues.ToString();
        }
        if (string.IsNullOrEmpty(apiKey))
        {
            return AuthenticateResult.NoResult();
        }
        var valid = await _apiKeyValidatorService.IsValidAsync(apiKey, Context.RequestAborted);
        if (!valid) {
            return AuthenticateResult.Fail("Invalid API Key");
        }
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, "apikey"),
            new (ClaimTypes.Name, "ApiKeyClient"),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
