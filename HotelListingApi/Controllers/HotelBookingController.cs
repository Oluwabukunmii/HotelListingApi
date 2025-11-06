using AutoMapper;
using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using HotelListingApi.DTOs.BookingsDtos;
using HotelListingApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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

        public HotelBookingController(IBookingService bookingService, IMapper mapper)
        {
            this.bookingService = bookingService;
            this.mapper = mapper;
        }

        // GET: api/hotels/{hotelId}/bookings
        [HttpGet]
        [Authorize]

        public async Task<IActionResult> GetAllBookings(int hotelId)
        {

            //Get loggedin user

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await bookingService.GetAllBookingsByHotelAsync(hotelId, userId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);


            // Map the Booking list to BookingDto list
            var mappedResult = result.Map(list => mapper.Map<List<BookingDto>>(list));  //list == result.Value , result.Value is a List<Booking>


            // Return 200 OK if success, proper error if failure
            return ToActionResult(mappedResult);
        }

        // GET: api/hotels/{hotelId}/bookings/{id}
        [HttpGet("{id}")]
        [Authorize]

        public async Task<IActionResult> GetBooking(int hotelId, int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await bookingService.GetBookingByIdAsync(id, userId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);


            var mappedResult = result.Map(b => mapper.Map<BookingDto>(b));
            //inline function , wherever you call b return thr mapper dto

            return ToActionResult(mappedResult);
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
                return ToActionResult(result);

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
        public async Task<IActionResult> PatchBooking(int hotelId, int id, [FromBody] JsonPatchDocument<UpdateBookingDto> patchDoc)
        {
            if (patchDoc == null)
                return ToActionResult(Result.Failure(new Error(ErrorTypes.BadRequest, "Invalid patch document.")));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return ToActionResult(Result.Failure(new Error(ErrorTypes.Forbid, "User not identified.")));

            // ✅ Delegate all business logic to the service
            var Patchedresult = await bookingService.PatchBookingAsync(id, patchDoc);

            // 🧠 Handle response conversion centrally via BaseApiController
            if (!Patchedresult.IsSuccess)
                return ToActionResult(Patchedresult);

            var updatedBookingDto = mapper.Map<BookingDto>(Patchedresult.Value);
            return ToActionResult(Result.Success(updatedBookingDto));
        }

      // For results that return a value (e.g. BookingDto)
        private IActionResult ToActionResult<T>(Result<T> result)
{
    if (result.IsSuccess)
        return Ok(result.Value); // ✅ return the actual object

    if (result.Errors != null && result.Errors.Any())
        return BadRequest(result.Errors);

    return StatusCode(500, "An unexpected error occurred.");
}

// For results without a value (e.g. Delete, Cancel, etc.)
private IActionResult ToActionResult(Result result)
{
    if (result.IsSuccess)
        return Ok(); // ✅ Just return 200 OK with no message

    if (result.Errors != null && result.Errors.Any())
        return BadRequest(result.Errors);

    return StatusCode(500, "An unexpected error occurred.");
}


    }
}
