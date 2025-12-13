using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain.Paging;
using HotelListingApi.DTOs.BookingsDtos;
using Microsoft.AspNetCore.JsonPatch;

namespace HotelListingApi.Interfaces
{
    public interface IBookingService
    {
        Task<Result<PaginationResult<BookingDto>>> GetAllBookingsByHotelAsync(int hotelId , string applicationUserId, paginationParameters paginationParameters);

        Task<Result<BookingDto>> GetBookingByIdAsync(int id, string applicationUserId);

        Task<Result<Booking>> CreateBookingAsync(Booking booking, string applicationUserId);

        Task<Result> UpdateBookingAsync(int id, Booking booking);

        Task<Result> CancelBookingAsync(int id);

        Task<Result<BookingDto>> PatchBookingAsync(int id, string applicationUserId, JsonPatchDocument<UpdateBookingDto> patchDoc);
    }
}
