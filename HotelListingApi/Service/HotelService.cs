using System.Diagnostics.Metrics;
using System.Linq.Dynamic.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListingApi.Common;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Extensions;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain.Models.Filtering;
using HotelListingApi.Domain.Paging;
using HotelListingApi.DTOs.HotelDtos;
using HotelListingApi.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Service
{
    public class HotelService(HotelListDbContext dbContext, IMapper mapper) : IHotelService
    {
        public async Task<Hotel> CreateAsync(Hotel hotel)
        {

            await dbContext.Hotels.AddAsync(hotel);
            await dbContext.SaveChangesAsync();
            return hotel;


        }

        public async Task<Hotel> DeleteAsync(int id)
        {
            var hotel = await dbContext.Hotels.FindAsync(id);


            if (hotel == null)
            {
                return null;
            }

            dbContext.Hotels.Remove(hotel);

            dbContext.Entry(hotel).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();
            return hotel;
        }

        public async Task<Result<PaginationResult<HotelDto>>> GetAllAsync(paginationParameters paginationParameters, HotelFilterParameter filters)
        {

            var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    { "id", "Id" },      // client can use ?sortBy=id
    { "name", "Name" },       // client can use ?sortBy=name
    { "rating", "Rating" },   // client can use ?sortBy=rating
    { "city", "City" }        // client can use ?sortBy=city
};

            var query = dbContext.Hotels.AsQueryable();
            if (filters.CountryId.HasValue)
            {
                query = query.Where(q => q.CountryId == filters.CountryId);
            }

            if (filters.MinRating.HasValue)
                query = query.Where(h => h.Rating >= filters.MinRating);

            if (filters.MaxRating.HasValue)
                query = query.Where(h => h.Rating <= filters.MaxRating);

            if (filters.MinPrice.HasValue)
                query = query.Where(h => h.PricePerNight >= filters.MinPrice);

            if (filters.MaxPrice.HasValue)
                query = query.Where(h => h.PricePerNight <= filters.MaxPrice);

            if (!string.IsNullOrWhiteSpace(filters.Location))
                query = query.Where(h => h.Address.Contains(filters.Location));

            // generic search param
            if (!string.IsNullOrWhiteSpace(filters.Search))
                query = query.Where(h => h.Name.Contains(filters.Search) ||
                                        h.Address.Contains(filters.Search));

            query = filters.SortBy?.ToLower() switch
            {
                "name" => filters.SortDescending ?
                    query.OrderByDescending(h => h.Name) : query.OrderBy(h => h.Name),
                "rating" => filters.SortDescending ?
                    query.OrderByDescending(h => h.Rating) : query.OrderBy(h => h.Rating),
                "price" => filters.SortDescending ?
                    query.OrderByDescending(h => h.PricePerNight) : query.OrderBy(h => h.PricePerNight),
                _ => query.OrderBy(h => h.Name)
            };

            var hotels = await query
            .Include(q => q.Country)
                .ProjectTo<HotelDto>(mapper.ConfigurationProvider)
                .ToPaginationResultAsync(paginationParameters, allowedSortColumns,
    defaultSortColumn: "Id");

            return Result<PaginationResult<HotelDto>>.Success(hotels);


            //return await dbContext.Hotels.ToListAsync();


        }

        public async Task<Hotel> GetByIdAsync(int id)
        {
            var hotel = await dbContext.Hotels.Include(h => h.Country).FirstOrDefaultAsync(h => h.Id == id);            //eager loading the country navigation property

            if (hotel == null)
            {
                return null;
            }

            return hotel;
        }

        public async Task<Hotel> UpdateAsync(int id, Hotel hotel)
        {
           
            // Find existing hotel
            var existingHotel = await dbContext.Hotels.FindAsync(id);

            if (existingHotel == null)
            {
                return null; // Not found
            }

            // Update properties
            existingHotel.Name = hotel.Name;
            existingHotel.Address = hotel.Address;
            existingHotel.Rating = hotel.Rating;
            existingHotel.CountryName = hotel.CountryName;
            existingHotel.CountryId = hotel.CountryId;

            dbContext.Entry(existingHotel).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();

            return existingHotel;

        }
    }


}



