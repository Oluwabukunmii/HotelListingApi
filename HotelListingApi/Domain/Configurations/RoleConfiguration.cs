using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingApi.Domain.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    ConcurrencyStamp = "d6e8f9a5-7d3a-4b10-b8da-0b2dc20e1c1a"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "HotelAdmin",
                    NormalizedName = "HOTELADMIN",
                    ConcurrencyStamp = "4d5e3b9c-8a1f-4e5a-bf9d-53c7a68ed019"
                },
                new IdentityRole
                {
                    Id = "3",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "77ccffb4-6b6e-4b77-8c0a-75fcd2186f88"
                }
            );
        }
    }
}
