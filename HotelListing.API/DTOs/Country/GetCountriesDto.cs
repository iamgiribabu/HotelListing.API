using HotelListing.API.DTOs.Hotel;

namespace HotelListing.API.DTOs.Country;

public record GetCountriesDto (int CountryId, string Name, string ShortName, List<GetHotelSlimDto> Hotel);
