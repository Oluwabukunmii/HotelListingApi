using HotelListingApi.Domain.Models;

namespace HotelListingApi.Interfaces
{
    public interface IHotelService
    {
        Task <Hotel> CreateAsync(Hotel hotel);

        Task<Hotel> UpdateAsync(int id, Hotel hotel);   

        Task<Hotel> DeleteAsync(int id);

        Task<Hotel> GetByIdAsync(int id);


        Task<List<Hotel>> GetAllAsync();



    }
}
