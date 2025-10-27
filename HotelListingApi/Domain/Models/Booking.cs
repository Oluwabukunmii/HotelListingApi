using HotelListingApi.Domain.Enum;

namespace HotelListingApi.Domain.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // ✅ Required fields (Check-in and Check-out)
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }

        // ✅ Default value must be an enum, not string
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;

        // ✅ Foreign Keys
        public string ApplicationUserId { get; set; } = string.Empty;
        public int HotelId { get; set; }

        // ✅ Navigation Properties
        public identityUser? ApplicationUser { get; set; }
        public Hotel? Hotel { get; set; }

        // Optional — useful for audit tracking
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
