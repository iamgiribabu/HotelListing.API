using HotelListing.API.DTOs.Country;
using HotelListing.API.Results;

namespace HotelListing.API.InterFace
{
    public interface ICountriesService
    {
        Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync();
        Task<Result<GetCountryDto>> GetCountryAsyncById(int id);
        Task<bool> UpdateCountryAsync(int id, UpdateCountryDto countryDto);
        Task<Result<int>> AddCountryAsync(CreateCountryDto countryDto);
        Task<bool> DeleteCountry(int id);
    }
}