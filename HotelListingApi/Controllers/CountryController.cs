using AutoMapper;
using HotelListingApi.Common;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain.Models.Filtering;
using HotelListingApi.Domain.Paging;
using HotelListingApi.DTOs.CountryDtos;
using HotelListingApi.Interfaces;
using HotelListingApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting(RateLimitingConstants.FixedPolicy)]

public class CountryController(HotelListDbContext dbContext, ICountryService countryService, IMapper mapper) : BaseApiController
{
    [HttpGet]
    [OutputCache(PolicyName = CacheConstants.AuthenticatedUserCachingPolicy)]
    [Authorize(Roles = "Administrator")]

    public async Task<ActionResult<PaginationResult<CountryListDto>>> GetAllAsync([FromQuery] paginationParameters paginationParameters, [FromQuery] CountryFilterParameter? filters)
    {
        var result = await countryService.GetAllAsync(paginationParameters, filters);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return ToActionResult(result);

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
    [Authorize(Roles = "Administrator")]

    public async Task<ActionResult<Country>> CreateAsync(CreateCountryDto createCountryDto)
    {
        var country = mapper.Map<Country>(createCountryDto);

        await countryService.CreateAsync(country);

        var result = mapper.Map<CountryDto>(country);

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]

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
    [Authorize(Roles = "Administrator")]

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



//Do a patch endpoint

