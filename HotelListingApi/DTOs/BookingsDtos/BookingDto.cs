namespace HotelListingApi.DTOs.BookingsDtos
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public double TotalAmount { get; set; }
    }

}
