namespace HotelListingApi.Domain.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string BookingStatus { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

        // Foreign Keys
        public string ApplicationUserId { get; set; } = string.Empty;
        public int HotelId { get; set; }

        // Navigation properties
        public ApplicationUser? ApplicationUser { get; set; }
        public Hotel? Hotel { get; set; }
    }
}
