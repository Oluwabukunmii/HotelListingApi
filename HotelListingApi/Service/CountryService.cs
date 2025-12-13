using AutoMapper.QueryableExtensions;
using AutoMapper;
using HotelListingApi.Common;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain.Paging;
using HotelListingApi.DTOs.BookingsDtos;
using HotelListingApi.DTOs.CountryDtos;
using HotelListingApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using HotelListingApi.Domain.Extensions;
using HotelListingApi.Domain.Models.Filtering;

namespace HotelListingApi.Service;

public class CountryService(HotelListDbContext dbContext , IMapper mapper) : ICountryService
{
    public async Task<Country> CreateAsync(Country country)
    {
        await dbContext.Countries.AddAsync(country);
        await dbContext.SaveChangesAsync();
        return country;
    }

    public async Task<Country> DeleteAsync(int countryId)
    {
        var country = await dbContext.Countries.FindAsync(countryId); //FindAsync because i only need to find by primary key

        if (country == null)
        {
            return null;
        }

        dbContext.Countries.Remove(country);
        dbContext.Entry(country).State = EntityState.Modified;

        await dbContext.SaveChangesAsync();
        return country;
    }

    public async Task<Result<PaginationResult<CountryListDto>>> GetAllAsync(paginationParameters paginationParameters, CountryFilterParameter? filters)
    {


var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) //create new dictionary in memory , maping user input to what i have in my dto
    {
        { "id", "CountryId" },
        { "name", "Name" },
        { "ShortName" , "ShortName" }
    };

        var query = dbContext.Countries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters?.Search))
        {
            var term = filters.Search.Trim();
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{term}%")
            || EF.Functions.Like(c.ShortName, $"%{term}%"));
        }


        var countries = await query
                     .AsNoTracking()
                   //.Include(c => c.Hotels)
                     .ProjectTo<CountryListDto>(mapper.ConfigurationProvider)
                     .ToPaginationResultAsync(paginationParameters, allowedSortColumns,
                   defaultSortColumn: "CountryId");


        return Result<PaginationResult<CountryListDto>>.Success(countries);

    }

    public async Task<Country> GetByIdAsync(int CountryId)
    {
             var country = await dbContext.Countries
            .Include(c => c.Hotels)
            .FirstOrDefaultAsync(c => c.CountryId == CountryId );

        return country;
    }

    public async Task<Country> UpdateAsync(int countryId, Country country)
    {
        var existingCountry = await dbContext.Countries.FindAsync(countryId);

   

        if (existingCountry == null)
        {
            return null;
        }

        existingCountry.Name = country.Name;
        existingCountry.ShortName = country.ShortName;

       // dbContext.Entry(existingCountry).State = EntityState.Modified; // Tracks the change


        await dbContext.SaveChangesAsync();

        return existingCountry;
    }
}
