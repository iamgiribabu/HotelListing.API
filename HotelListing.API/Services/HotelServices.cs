using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Data;
using HotelListing.API.DTOs.Hotel;
using HotelListing.API.InterFace;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Services;

public class HotelServices : IHotelServices
{
    private readonly HotelListingDbContext _context;
    private readonly IMapper _mapper;
    public HotelServices(HotelListingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<IEnumerable<GetHotelsDto>> GetAllHotelsAsync()
    {
        var hotels = await _context.Hotels
            .Select(h => new GetHotelsDto(h.id, h.Name, h.Address, h.Rating, h.CountryId))
            .ToListAsync();
        return hotels;
    }

    public async Task<GetHotelDto?> GetHotelByIdAsync(int id)
    {
        var hotel = await _context.Hotels
            .Where(x => x.id == id)
            .Include(q => q.Country)
            .ProjectTo<GetHotelDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        return hotel;
    }

    public async Task<bool> UpdateHotelAsync(int id, UpdateHotelDto hotelDto)
    {
        var hotel = await _context.Hotels.FindAsync(id);

        if (hotel == null)
        {
            throw new Exception($"No Data found for Hote iD {id}");
        }

        hotel.Name = hotelDto.Name;
        hotel.Address = hotelDto.Address;
        hotel.Rating = hotelDto.Rating;
        hotel.CountryId = hotelDto.CountryId;

        _context.Entry(hotel).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<GetHotelDto> AddHotelAsync(CreateHotelDto hotelDto)
    {
        var countryExists = await _context.Countries.AnyAsync(c => c.CountryId == hotelDto.CountryId);
        if (!countryExists)
        {
            throw new Exception($"Country with ID {hotelDto.CountryId} does not exist");
        }

        var hotel = _mapper.Map<Hotel>(hotelDto);

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();
        var hotelObj = _mapper.Map<GetHotelDto>(hotel);
        return hotelObj;
    }

    public async Task<bool> DeleteHotelAsync(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
        {
            throw new Exception($"No Hotel Found for Id {id}");
        }

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
        return true;
    }
}
