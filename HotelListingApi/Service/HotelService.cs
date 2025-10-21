using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Service
{
    public class HotelService(HotelListDbContext dbContext) : IHotelService
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
           await dbContext.SaveChangesAsync();

            return hotel;
        }

        public async Task<List<Hotel>> GetAllAsync()
        {
            return await dbContext.Hotels.ToListAsync();


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

          await dbContext.SaveChangesAsync();

            return existingHotel;

        }
    }


}



