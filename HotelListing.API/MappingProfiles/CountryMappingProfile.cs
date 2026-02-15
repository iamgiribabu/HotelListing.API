using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.DTOs.Country;

namespace HotelListing.API.MappingProfiles;

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountryDto>()
            .ForMember(d => d.Hotel, cfg => cfg.MapFrom(s => s.Hotels));
        CreateMap<Country, GetCountriesDto>();
        CreateMap<CreateCountryDto, Country>();
    }
}