public class CreateBookingDto
{
    public int HotelId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    //  public int NumberOfGuests { get; set; }
    // public double TotalPrice{ get; set; }

    public int NumberOfGuests { get; set; }
    public double TotalPrice { get; set; }

    public string BookingStatus { get; set; } = "Pending";
}