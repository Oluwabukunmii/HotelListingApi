using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace HotelListingApi.Interfaces
{
    public interface IBookingService
    {
        Task<Result<List<Booking>>> GetAllBookingsByHotelAsync(int hotelId , string applicationUserId);

        Task<Result<Booking>> GetBookingByIdAsync(int id, string applicationUserId);

        Task<Result<Booking>> CreateBookingAsync(Booking booking, string applicationUserId);

        Task<Result> UpdateBookingAsync(int id, Booking booking);

        Task<Result> CancelBookingAsync(int id);

        Task<Result<Booking>> PatchBookingAsync(int id, JsonPatchDocument<UpdateBookingDto> patchDoc);
    }
}
