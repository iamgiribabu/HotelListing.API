using HotelListing.API.Data;
using HotelListing.API.InterFace;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Services;

public class ApiKeyValidatorService : IApiKeyValidatorService
{
    private readonly HotelListingDbContext _db;
    public ApiKeyValidatorService(HotelListingDbContext db)
    {
        _db = db;
    }
    public async Task<bool> IsValidAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key)) return false;
        var apiKeyEntity = await _db.ApiKeys.AsNoTracking().FirstOrDefaultAsync(x => x.Key == key, cancellationToken);   
        if (apiKeyEntity == null) return false;
        return apiKeyEntity.IsActive;
    }
}
