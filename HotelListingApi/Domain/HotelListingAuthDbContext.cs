using HotelListingApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HotelListingApi.Domain.Configurations;
using HotelListingApi.Domain.Models;
using System.Data;
using System.Security.Claims;

namespace HotelListingApi.Domain
{
    public class HotelListingAuthDbContext : IdentityDbContext<IdentityUser>//IdentityDbContext<IdentityUser> comes from ASP.NET Core Identity and provides built-in tables for:Users (AspNetUsers)Roles (AspNetRoles)UserRoles (AspNetUserRoles)Claims, Logins, Tokens, etc.
    {
        public HotelListingAuthDbContext(DbContextOptions<HotelListingAuthDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply any additional entity configurations (like Role seeding)
            builder.ApplyConfiguration(new RoleConfiguration());
            // Optional — add indexes 
            builder.Entity<IdentityRole>()
                .HasIndex(r => r.Name)
                .HasDatabaseName("IX_IdentityRoles_Name");
        }
    }
    
}


