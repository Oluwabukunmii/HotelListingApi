using AutoMapper;
using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain.Paging;
using HotelListingApi.DTOs.BookingsDtos;
using HotelListingApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace HotelListingApi.Controllers
{
    [Route("api/hotels/{hotelId:int}/bookings")]
    [ApiController]
    [EnableRateLimiting(RateLimitingConstants.PerUserPolicy)]


    public class HotelBookingController : BaseApiController
    {
        private readonly IBookingService bookingService;
        private readonly IMapper mapper;

        public HotelBookingController(IBookingService bookingService, IMapper mapper)
        {
            this.bookingService = bookingService;
            this.mapper = mapper;
        }

        // GET: api/hotels/{hotelId}/bookings
        [HttpGet]
        [Authorize]

        public async Task<ActionResult<PaginationResult<BookingDto>>> GetAllBookings([FromRoute] int hotelId,
            [FromQuery] paginationParameters paginationParameters)

        {

            //Get loggedin user

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await bookingService.GetAllBookingsByHotelAsync(hotelId, userId, paginationParameters);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);


            // Return 200 OK if success, proper error if failure
            return  ToActionResult(result);
        }

        // GET: api/hotels/{hotelId}/bookings/{id}
        [HttpGet("{id}")]
        [Authorize]

        public async Task<ActionResult<BookingDto>> GetBooking(int hotelId, int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await bookingService.GetBookingByIdAsync(id, userId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);



            return ToActionResult(result);
        }

        // POST: api/hotels/{hotelId}/bookings
        [HttpPost]
        [Authorize]

        public async Task<IActionResult> CreateBooking(int hotelId, [FromBody] CreateBookingDto bookingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = mapper.Map<Booking>(bookingDto);
            booking.HotelId = hotelId;

            var result = await bookingService.CreateBookingAsync(booking, userId);

            if (!result.IsSuccess)
                return ToActionResult(Result.Failure(result.Errors));


            var createdBookingDto = mapper.Map<BookingDto>(result.Value);

            return CreatedAtAction(
                nameof(GetBooking),
                new { hotelId, id = createdBookingDto.Id },
                createdBookingDto
            );
        }

        // PUT: api/hotels/{hotelId}/bookings/{id}
        [HttpPut("{id}")]
        [Authorize]

        public async Task<IActionResult> UpdateBooking(int hotelId, int id, [FromBody] UpdateBookingDto updateBookingDto)
        {
            var booking = mapper.Map<Booking>(updateBookingDto);
            booking.Id = id;
            booking.HotelId = hotelId;

            var result = await bookingService.UpdateBookingAsync(id, booking);


            if (!result.IsSuccess)
                return ToActionResult(Result.Failure(result.Errors));

            // var mappedResult = result.Map(b => mapper.Map<BookingDto>(b));
            //  return ToActionResult(mappedResult);

            return ToActionResult(result);






        }

        // DELETE: api/hotels/{hotelId}/bookings/{id}
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<IActionResult> CancelBooking(int hotelId, int id)
        {
            var result = await bookingService.CancelBookingAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok("Booking successfully deleted");
        }


        [HttpPatch("{id:int}")]
        [Authorize]
        public async Task<ActionResult<BookingDto>> PatchBooking(int hotelId, int id, [FromBody] JsonPatchDocument<UpdateBookingDto> patchDoc)
        {
            if (patchDoc == null)
                return ToActionResult(Result.Failure(new Error(ErrorTypes.BadRequest, "Invalid patch document.")));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return ToActionResult(Result.Failure(new Error(ErrorTypes.Forbid, "User not identified.")));

            // Delegate all business logic to the service
            var Patchedresult = await bookingService.PatchBookingAsync(id, userId, patchDoc);

            // Handle response conversion centrally via BaseApiController
            if (!Patchedresult.IsSuccess)
                return ToActionResult(Result.Failure(Patchedresult.Errors));


            return ToActionResult(Patchedresult);
        }

       
             
    }

}
