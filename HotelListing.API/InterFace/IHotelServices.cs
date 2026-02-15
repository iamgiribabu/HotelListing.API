using HotelListing.API.Data;
using HotelListing.API.DTOs.Hotel;

namespace HotelListing.API.InterFace
{
    public interface IHotelServices
    {
        Task<GetHotelDto> AddHotelAsync(CreateHotelDto hotelDto);
        Task<bool> DeleteHotelAsync(int id);
        Task<IEnumerable<GetHotelsDto>> GetAllHotelsAsync();
        Task<GetHotelDto?> GetHotelByIdAsync(int id);
        Task<bool> UpdateHotelAsync(int id, UpdateHotelDto hotelDto);
    }
}