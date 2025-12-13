using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain.Models.Filtering;
using HotelListingApi.Domain.Paging;
using HotelListingApi.DTOs.CountryDtos;

namespace HotelListingApi.Interfaces;

public interface ICountryService
{
    Task<Result<PaginationResult<CountryListDto>>> GetAllAsync(paginationParameters paginationParameters, CountryFilterParameter? f);
    Task<Country> GetByIdAsync(int id);
    Task<Country> CreateAsync(Country country);
    Task<Country> UpdateAsync(int id, Country country);
    Task<Country> DeleteAsync(int id);
}
