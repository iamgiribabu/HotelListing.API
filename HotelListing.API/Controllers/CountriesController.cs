using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.DTOs.Country;
using HotelListing.API.DTOs.Hotel;
using HotelListing.API.InterFace;
using Microsoft.AspNetCore.Authorization;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CountriesController : ControllerBase
{
    private readonly ICountriesService _countriesService;

    public CountriesController(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var countries = await _countriesService.GetCountriesAsync();
        return Ok(countries.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
       var country = await _countriesService.GetCountryAsyncById(id);

        if (country.Value == null)
        {
            return NotFound();
        }

        return Ok(country.Value);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto countryDto)
    {
        if (id != countryDto.CountryId)
        {
            return BadRequest();
        }

        try
        {
           await _countriesService.UpdateCountryAsync(id, countryDto);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return NotFound(ex.Message);
        }

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto countryDto)
    {
       var insertedCountryId =  await _countriesService.AddCountryAsync(countryDto);
        var resultDto = new GetCountryDto(insertedCountryId.Value, countryDto.Name, countryDto.ShortName, new List<GetHotelSlimDto>());

        return CreatedAtAction("GetCountry", new { id = insertedCountryId }, resultDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        await _countriesService.DeleteCountry(id);

        return NoContent();
    }

  
}
