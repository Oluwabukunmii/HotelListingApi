using AutoMapper;
using HotelListingApi.Common;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain.Models.Filtering;
using HotelListingApi.Domain.Paging;
using HotelListingApi.DTOs.HotelDtos;
using HotelListingApi.Interfaces;
using HotelListingApi.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting(RateLimitingConstants.FixedPolicy)]

public class HotelController(HotelListDbContext dbContext, IHotelService hotelService, IMapper mapper) : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<PaginationResult<HotelDto>>> GetAllAsync([FromQuery]paginationParameters paginationParameters, [FromQuery]HotelFilterParameter filters)
    {
        var hotelResult = await hotelService.GetAllAsync(paginationParameters, filters);


        return ToActionResult(hotelResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> GetByIdAsync(int id)
    {

        var hotel = await hotelService.GetByIdAsync(id);

        if (hotel == null)
        {
            return NotFound();

        }

        var result = mapper.Map<HotelDto>(hotel);

        return Ok(result);




    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, CreateHotelDto CreateHotelDto)
    {

        var hoteldomain = mapper.Map<Hotel>(CreateHotelDto);

        var Hotel = await hotelService.UpdateAsync(id, hoteldomain);


        if (Hotel == null)
        {
            return NotFound();

        }

        var result = mapper.Map<HotelDto>(Hotel);

        return Ok(result);



    }


    [HttpPost]
    public async Task<ActionResult<Hotel>> CreateAsync(CreateHotelDto CreateHotelDto)

    {

        var hotel = mapper.Map<Hotel>(CreateHotelDto);


        await hotelService.CreateAsync(hotel);


        return Ok(mapper.Map<HotelDto>(hotel));



    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {

        var hotel = await hotelService.DeleteAsync(id);

        if (hotel == null)
        {
            return NotFound();

        }

        mapper.Map<HotelDto>(hotel);

        return Ok("Hotel successfully deleted");
    }

    /* private bool HotelExists(int id)
     {
         return context.Hotels.Any(e => e.Id == id);
     }*/
}
