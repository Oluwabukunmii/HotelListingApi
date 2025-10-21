using AutoMapper;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.DTOs.CountryDtos;
using HotelListingApi.Interfaces;
using HotelListingApi.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController(HotelListDbContext dbContext, ICountryService countryService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Country>> GetAllAsync()
    {
        var countries = await countryService.GetAllAsync();
        var result = mapper.Map<List<CountryDto>>(countries);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> GetByIdAsync(int id)
    {
        var country = await countryService.GetByIdAsync(id);

        if (country == null)
        {
            return NotFound();
        }

        var result = mapper.Map<CountryDto>(country);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Country>> CreateAsync(CreateCountryDto createCountryDto)
    {
        var country = mapper.Map<Country>(createCountryDto);

        await countryService.CreateAsync(country);

        var result = mapper.Map<CountryDto>(country);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, CreateCountryDto createCountryDto)
    {
        var country = mapper.Map<Country>(createCountryDto);

        var updatedCountry = await countryService.UpdateAsync(id, country);

        if (updatedCountry == null)
        {
            return NotFound();
        }

        var result = mapper.Map<CountryDto>(updatedCountry);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deletedCountry = await countryService.DeleteAsync(id);

        if (deletedCountry == null)
        {
            return NotFound();
        }

        return Ok("Country successfully deleted");
    }
}





