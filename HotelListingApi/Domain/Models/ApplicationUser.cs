using Microsoft.AspNetCore.Identity;

namespace HotelListingApi.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; }

        public string Password { get; set; }
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;

        // Navigation - One user can have many bookings
        public ICollection<Booking>? Bookings { get; set; }
    }
}
