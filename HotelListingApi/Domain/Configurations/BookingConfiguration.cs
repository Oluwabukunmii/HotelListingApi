using HotelListingApi.Domain.Enum;
using HotelListingApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingApi.Domain.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            // Table name
            builder.ToTable("Bookings");

            // Primary key
            builder.HasKey(b => b.Id);

            // Relationships
            builder.HasOne(b => b.Hotel)
                   .WithMany(h => h.Bookings)
                   .HasForeignKey(b => b.HotelId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.ApplicationUser)
                   .WithMany(u => u.Bookings)
                   .HasForeignKey(b => b.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Property configurations
            builder.Property(b => b.CheckInDate)
                   .IsRequired();

            builder.Property(b => b.CheckOutDate)
                   .IsRequired();

            builder.Property(b => b.BookingStatus)
                   .HasMaxLength(20)
                   .HasDefaultValue(BookingStatus.Pending)
                    .HasConversion<string>();


            // Optional: Add index for performance
            builder.HasIndex(b => new { b.HotelId, b.CheckInDate })
                   .HasDatabaseName("IX_Bookings_HotelId_CheckInDate");
        }
    }
}
