namespace HotelListing.API.InterFace
{
    public interface IApiKeyValidatorService
    {
        Task<bool> IsValidAsync(string key, CancellationToken cancellationToken = default);
    }
}