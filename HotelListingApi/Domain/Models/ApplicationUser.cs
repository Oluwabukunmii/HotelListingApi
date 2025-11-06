using Microsoft.AspNetCore.Identity;

namespace HotelListingApi.Domain.Models
{
    public class ApplicationUser
    {
        public string ApplicationUserId { get; set; }

        public string FullName { get; set; } 
         
        public string Email { get; set; }  


        public DateTime DateJoined { get; set; } = DateTime.UtcNow;

        // Navigation - One user can have many bookings
        public ICollection<Booking>? Bookings { get; set; }
    }
}
