using HotelListingApi.Domain.Models;

namespace HotelListingApi.Interfaces;

public interface ICountryService
{
    Task<List<Country>> GetAllAsync();
    Task<Country> GetByIdAsync(int id);
    Task<Country> CreateAsync(Country country);
    Task<Country> UpdateAsync(int id, Country country);
    Task<Country> DeleteAsync(int id);
}
