using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain.Models.Filtering;
using HotelListingApi.Domain.Paging;
using HotelListingApi.DTOs.HotelDtos;

namespace HotelListingApi.Interfaces
{
    public interface IHotelService
    {
        Task <Hotel> CreateAsync(Hotel hotel);

        Task<Hotel> UpdateAsync(int id, Hotel hotel);
        Task<Hotel> DeleteAsync(int id);
        Task<Hotel> GetByIdAsync(int id);

        Task<Result<PaginationResult<HotelDto>>>GetAllAsync(paginationParameters paginationParameters, HotelFilterParameter filters);




    }
}
