using AutoMapper;
using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using HotelListingApi.DTOs.BookingsDtos;
using HotelListingApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelListingApi.Controllers
{
    [Route("api/hotels/{hotelId:int}/bookings")]
    [ApiController]
    public class HotelBookingController : BaseApiController
    {
        private readonly IBookingService bookingService;
        private readonly IMapper mapper;

        public HotelBookingController(IBookingService bookingService, IMapper mapper )
        {
            this.bookingService = bookingService;
            this.mapper = mapper;
        }

        // GET: api/hotels/{hotelId}/bookings
        [HttpGet]
        public async Task<IActionResult> GetAllBookings(int hotelId)
        {

            //Get loggedin user

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await bookingService.GetAllBookingsByHotelAsync(hotelId, userId);

            // Map the Booking list to BookingDto list
            var mappedResult = result.Map(list => mapper.Map<List<BookingDto>>(list));  //list == result.Value , result.Value is a List<Booking>


            // Return 200 OK if success, proper error if failure
            return ToActionResult(mappedResult);
        }

        // GET: api/hotels/{hotelId}/bookings/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int hotelId, int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await bookingService.GetBookingByIdAsync(id, userId);

            var mappedResult = result.Map(b => mapper.Map<BookingDto>(b));//inline function , wherever you call b return thr mapper dto

            return ToActionResult(mappedResult);
        }

        // POST: api/hotels/{hotelId}/bookings
        [HttpPost]
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
        public async Task<IActionResult> UpdateBooking(int hotelId, int id, [FromBody] UpdateBookingDto bookingDto)
        {
            var booking = mapper.Map<Booking>(bookingDto);
            booking.Id = id;
            booking.HotelId = hotelId;

            var result = await bookingService.UpdateBookingAsync(id, booking);

            // UpdateAsync returns Result (no value), ToActionResult handles success/errors
            return ToActionResult(result);
        }

        // DELETE: api/hotels/{hotelId}/bookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int hotelId, int id)
        {
            var result = await bookingService.CancelBookingAsync(id);
            return ToActionResult(result);
        }
        // Update the return type of ToActionResult to ensure compatibility with IActionResult
        private IActionResult ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Errors != null && result.Errors.Any())
            {
                return BadRequest(result.Errors);
            }

            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}
