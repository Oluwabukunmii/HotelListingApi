using HotelListingApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HotelListingApi.Domain.Configurations;
using HotelListingApi.Domain.Models;

namespace HotelListingApi.Domain
{
    public class HotelListingAuthDbContext : IdentityDbContext<identityUser>
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


