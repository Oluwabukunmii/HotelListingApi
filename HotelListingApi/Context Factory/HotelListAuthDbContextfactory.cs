using HotelListingApi.Domain;
using HotelListingApi.Domain.Models; // where HotelListingAuthDbContext is defined
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HotelListingApi.Context_Factory
{
    public class HotelListingAuthDbContextFactory : IDesignTimeDbContextFactory<HotelListingAuthDbContext>
    {
        public HotelListingAuthDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Create options builder for Auth DbContext
            var optionsBuilder = new DbContextOptionsBuilder<HotelListingAuthDbContext>();
            var connectionString = configuration.GetConnectionString("HotelListingAuthConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            // Return instance
            return new HotelListingAuthDbContext(optionsBuilder.Options);
        }
    }
}
