using AutoMapper;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.DTOs.CountryDtos;
using HotelListingApi.Interfaces;
using HotelListingApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingApi.Controllers
{
    [Route("api/v{version:apiVersion}/country")]
    [ApiController]
    [ApiVersion("2.0")]

    public class CountryV2Controller(HotelListDbContext dbContext, ICountryService countryService, IMapper mapper) : BaseApiController
    {
        [HttpGet("{id}")]

        public async Task<ActionResult<Country>> GetByIdAsync(int id)
        {
            var country = await countryService.GetByIdAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            var result = mapper.Map<CountryV2Dto>(country);
            return Ok(result);
        }
    }
}
