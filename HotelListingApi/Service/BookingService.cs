using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain;
using HotelListingApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using HotelListingApi.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using HotelListingApi.Domain.Paging;
using HotelListingApi.Domain.Extensions;
using AutoMapper.QueryableExtensions;
using HotelListingApi.DTOs.BookingsDtos;
using Microsoft.Extensions.Caching.Memory;

namespace HotelListingApi.Services
{
    public class BookingService : IBookingService
    {
        private readonly HotelListDbContext dbContext;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public BookingService(HotelListDbContext dbContext, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<Result<PaginationResult<BookingDto>>> GetAllBookingsByHotelAsync(int hotelId, string applicationUserId,paginationParameters paginationParameters)
        {

  var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
   {
    { "id", "Id" },
    { "checkin", "CheckInDate" },
    { "checkout", "CheckOutDate" },
    { "created", "CreatedAt" }
};

            var hotelExists = await dbContext.Hotels.AnyAsync(h => h.Id == hotelId);
            if (!hotelExists)
                return Result<PaginationResult<BookingDto>>.Failure(
                    new Error(ErrorTypes.NotFound, "Hotel not found."));



            var bookings = await dbContext.Bookings
                .Include(b => b.Hotel)
                .Where(b => b.HotelId == hotelId)
                .Where(b => b.ApplicationUserId == applicationUserId)
                .ProjectTo<BookingDto>(mapper.ConfigurationProvider)
                .ToPaginationResultAsync(paginationParameters, allowedSortColumns,
    defaultSortColumn: "Id");

            return Result<PaginationResult<BookingDto>>.Success(bookings);


        }

        public async Task<Result<BookingDto>> GetBookingByIdAsync(int id, string applicationUserId)
        {
            var booking = await dbContext.Bookings
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == id && b.ApplicationUserId == applicationUserId);


            if (booking == null)
                return Result<BookingDto>.Failure(
                    new Error(ErrorTypes.NotFound, "Booking not found."));

            var bookingDto = mapper.Map<BookingDto>(booking);


            return Result<BookingDto>.Success(bookingDto);
        }

        public async Task<Result<Booking>> CreateBookingAsync(Booking booking, string applicationUserId)
        {
            if (booking == null)
                return Result<Booking>.BadRequest(
                    new Error(ErrorTypes.BadRequest, "Invalid booking data."));

           // if (booking.CheckInDate == default || booking.CheckOutDate == default)
               // return Result<Booking>.BadRequest(
                //    new Error(ErrorTypes.Validation, "Check-in and check-out dates are required."));

            if (booking.CheckInDate >= booking.CheckOutDate)
                return Result<Booking>.BadRequest(
                    new Error(ErrorTypes.Validation, "Check-in date must be earlier than check-out date."));

            var hotel = await dbContext.Hotels.FirstOrDefaultAsync(h => h.Id == booking.HotelId);
            if (hotel == null)
                return Result<Booking>.Failure(
                    new Error(ErrorTypes.NotFound, "Hotel not found."));

            var numberOfNights = (booking.CheckOutDate - booking.CheckInDate).TotalDays;
            booking.TotalPrice = (decimal)numberOfNights * hotel.PricePerNight;
            booking.ApplicationUserId = applicationUserId;
            booking.BookingStatus = BookingStatus.Pending;
            booking.CreatedAt = DateTime.UtcNow;

            await dbContext.Bookings.AddAsync(booking);
            await dbContext.SaveChangesAsync();

            return Result<Booking>.Success(booking);
        }

        public async Task<Result> UpdateBookingAsync(int id, Booking booking)
        {
            if (id != booking.Id)
                return Result.BadRequest(
                    new Error(ErrorTypes.BadRequest, "Booking ID mismatch."));

            var existingBooking = await dbContext.Bookings
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBooking == null)
                return Result.NotFound(
                    new Error(ErrorTypes.NotFound, "Booking not found."));

            if (existingBooking.BookingStatus ==BookingStatus.Cancelled)
                return Result.BadRequest(
                    new Error(ErrorTypes.BadRequest, "Cannot update a cancelled booking."));

            if (booking.CheckInDate >= booking.CheckOutDate)
                return Result.BadRequest(
                    new Error(ErrorTypes.Validation, "Check-in date must be earlier than check-out date."));

            var numberOfNights = (booking.CheckOutDate - booking.CheckInDate).TotalDays;
            existingBooking.TotalPrice = (decimal)numberOfNights * existingBooking.Hotel.PricePerNight;
            existingBooking.CheckInDate = booking.CheckInDate;
            existingBooking.CheckOutDate = booking.CheckOutDate;

            dbContext.Entry(existingBooking).State = EntityState.Modified;


            await dbContext.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> CancelBookingAsync(int id)
        {
            var booking = await dbContext.Bookings.FindAsync(id);
            if (booking == null)
                return Result.NotFound(
                    new Error(ErrorTypes.NotFound, "Booking not found."));

            dbContext.Bookings.Remove(booking);

            dbContext.Entry(booking).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();

            return Result.Success();
        }


        // ---- NEW: Patch implementation ----
        public async Task<Result<BookingDto>> PatchBookingAsync(int id, string applicationUserId, JsonPatchDocument<UpdateBookingDto> patchDoc)
        {
            if (patchDoc == null)
                return Result<BookingDto>.BadRequest(
                    new Error(ErrorTypes.BadRequest, "Invalid patch document."));

            var existingBooking = await dbContext.Bookings
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == id && b.ApplicationUserId == applicationUserId);


            if (existingBooking == null)
                return Result<BookingDto>.Failure(
                    new Error(ErrorTypes.NotFound, "Booking not found."));

            if (existingBooking.BookingStatus == BookingStatus.Cancelled)
                return Result<BookingDto>.BadRequest(
                    new Error(ErrorTypes.BadRequest, "Cannot modify a cancelled booking."));

            // Map the entity to UpdateBookingDto to apply patch
            var bookingToPatch = mapper.Map<UpdateBookingDto>(existingBooking);



         //   var updatedBookingDto = mapper.Map<BookingDto>(Patchedresult.Value);


            // Apply patch to the DTO
            patchDoc.ApplyTo(bookingToPatch);

            // Validate new data
            if (bookingToPatch.CheckInDate >= bookingToPatch.CheckOutDate)
                return Result<BookingDto>.BadRequest(
                    new Error(ErrorTypes.Validation, "Check-in date must be earlier than check-out date."));

            // Map back to entity
            mapper.Map(bookingToPatch, existingBooking);

            // Recalculate total price
            var numberOfNights = (existingBooking.CheckOutDate - existingBooking.CheckInDate).TotalDays;
            existingBooking.TotalPrice = (decimal)numberOfNights * existingBooking.Hotel.PricePerNight;

            dbContext.Entry(existingBooking).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();

            // Map entity to DTO to return
            var updatedBookingDto = mapper.Map<BookingDto>(existingBooking);


            return Result<BookingDto>.Success(updatedBookingDto);
        }

    }
}
    

