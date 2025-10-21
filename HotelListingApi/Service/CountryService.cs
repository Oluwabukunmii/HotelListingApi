using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Service;

public class CountryService(HotelListDbContext dbContext) : ICountryService
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
        await dbContext.SaveChangesAsync();

        return country;
    }

    public async Task<List<Country>> GetAllAsync()
    {
        return await dbContext.Countries
            .Include(c => c.Hotels)
            .ToListAsync();
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

        await dbContext.SaveChangesAsync();

        return existingCountry;
    }
}
