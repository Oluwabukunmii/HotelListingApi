using HotelListingApi.DTOs.BookingsDtos;

public class ApplicationUserDto
{
    public string fullName { get; set; }

    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<BookingDto>? Bookings { get; set; }

}