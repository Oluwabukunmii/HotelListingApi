using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingApi.Context_Factory
{
    public class HotelListDbContextFactory : IDesignTimeDbContextFactory<HotelListDbContext>
    {
        public HotelListDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HotelListDbContext>();
            var connectionString = configuration.GetConnectionString("HotelListingConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            return new HotelListDbContext(optionsBuilder.Options);
        }
    }
}
