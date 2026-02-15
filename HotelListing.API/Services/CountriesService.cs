using HotelListing.API.Data;
using HotelListing.API.DTOs.Country;
using HotelListing.API.DTOs.Hotel;
using HotelListing.API.InterFace;
using HotelListing.API.Results;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Services;

public class CountriesService : ICountriesService
{
    private readonly HotelListingDbContext _context;
    public CountriesService(HotelListingDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync()
    {
        var countries = await _context.Countries
            .Select(c => new GetCountriesDto(c.CountryId, c.Name, c.ShortName, c.Hotels.Select(h => new GetHotelSlimDto(
                    h.id, h.Name, h.Address, h.Rating)).ToList()))
            .ToListAsync();

        return Result<IEnumerable<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryAsyncById(int id)
    {
        var country = await _context.Countries
           .Where(x => x.CountryId == id)
           .Select(c => new GetCountryDto(c.CountryId, c.Name, c.ShortName,
               c.Hotels.Select(h => new GetHotelSlimDto(
                   h.id, h.Name, h.Address, h.Rating)).ToList()
           )).FirstOrDefaultAsync();
        return country is null 
            ? Result<GetCountryDto>.NotFound() 
            : Result<GetCountryDto>.Success(country);
    }

    public async Task<bool> UpdateCountryAsync(int id, UpdateCountryDto countryDto)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null)
        {
            throw new InvalidOperationException($"No Country Found for {id}");
        }
        country.Name = countryDto.Name;
        country.ShortName = countryDto.ShortName;

        _context.Entry(country).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<int>> AddCountryAsync(CreateCountryDto countryDto)
    {
        if(countryDto == null)
        {
            throw new ArgumentNullException(nameof(countryDto));
        }
        var country = new Country
        {
            Name = countryDto.Name,
            ShortName = countryDto.ShortName
        };
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
        return Result<int>.Success(country.CountryId);
    }

    public async Task<bool> DeleteCountry(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null)
        {
            throw new Exception($"No Country found for Id {id}");
        }

        _context.Countries.Remove(country);
        await _context.SaveChangesAsync();
        return true;
    }


}
