using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.DTOs.Hotel;
using HotelListing.API.InterFace;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IHotelServices _hotelServices;

    public HotelsController(IHotelServices hotelServices)
    {
        _hotelServices = hotelServices;
    }

    // GET: api/Hotels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetHotelsDto>>> GetHotels()
    {
        var hotels = await _hotelServices.GetAllHotelsAsync();
        return Ok(hotels);

    }

    // GET: api/Hotels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetHotelDto>> GetHotel(int id)
    {
        var hotel = await _hotelServices.GetHotelByIdAsync(id);

        if (hotel == null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutHotel(int id, UpdateHotelDto hotelDto)
    {
        if (id != hotelDto.Id)
        {
            return BadRequest();
        }

        try
        {
            await _hotelServices.UpdateHotelAsync(id, hotelDto);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

        return NoContent();
    }

    //[HttpPatch("{id}")]
    //public async Task<IActionResult> PatchHotel(int id, UpdateHotelDto hotelDto)
    //{
    //    var hotel = await _context.Hotels.FindAsync(id);

    //    if (hotel == null)
    //    {
    //        return NotFound();
    //    }

    //    if (!string.IsNullOrEmpty(hotelDto.Name))
    //    {
    //        hotel.Name = hotelDto.Name;
    //    }

    //    if (!string.IsNullOrEmpty(hotelDto.Address))
    //    {
    //        hotel.Address = hotelDto.Address;
    //    }

    //    if (hotelDto.Rating.HasValue)
    //    {
    //        if (hotelDto.Rating.Value < 0 || hotelDto.Rating.Value > 5)
    //        {
    //            return BadRequest("Rating must be between 0 and 5");
    //        }
    //        hotel.Rating = hotelDto.Rating.Value;
    //    }


    //    _context.Entry(hotel).State = EntityState.Modified;

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (!HotelExists(id))
    //        {
    //            return NotFound();
    //        }
    //        throw;
    //    }

    //    return NoContent();
    //}

    [HttpPost]
    public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto hotelDto)
    {
        if (hotelDto == null)
        {
            return BadRequest("Hotel Request is empty");
        }

        try
        {
            var hotel = await _hotelServices.AddHotelAsync(hotelDto);

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Database error occurred: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        await _hotelServices.DeleteHotelAsync(id);

        return NoContent();
    }



    
}
