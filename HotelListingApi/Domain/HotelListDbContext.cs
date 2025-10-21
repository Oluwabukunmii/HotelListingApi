using System.Reflection;
using HotelListingApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace HotelListingApi.Domain
{
    public class HotelListDbContext :   DbContext
    {
        public HotelListDbContext(DbContextOptions<HotelListDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }


        public DbSet<Booking> Bookings { get; set; }
        public DbSet<HotelAdmin> HotelAdmins { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)  //This method is the place for Fluent API model configuration. EF Core calls it during model building. You override it to apply conventions, indexes, relationships, seeding and to apply configuration classes.
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); //Scans the current assembly for any classes implementing IEntityTypeConfiguration<T> and applies each Configure method., (appy config for country,hotel.hoteladmin)

            // Many-to-many: Hotel <-> HotelAdmin //could be done automatically by ef core but just for readability
            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.HotelAdmins)
                .WithMany(a => a.Hotels)
                .UsingEntity(j => j.ToTable("HotelAdmins_Hotels"));

            // Indexes for performance
            modelBuilder.Entity<Hotel>()
                .HasIndex(h => h.Name) //Creates a database index on the Name column of the Hotels table.
                .HasDatabaseName("IX_Hotels_Name");//gives the index a stable, readable name (IX_Hotels_Name)

            //queries that filter or sort by Name will be faster because the DB can look up rows via the index instead of scanning the entire table.

            modelBuilder.Entity<Hotel>()
                .HasIndex(h => new { h.CountryId, h.Rating }) //Adds a composite index across two columns: CountryId and Rating.
                .HasDatabaseName("IX_Hotels_CountryId_Rating");
        }



    }
}
