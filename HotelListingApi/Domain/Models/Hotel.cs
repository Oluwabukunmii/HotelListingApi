namespace HotelListingApi.Domain.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public string CountryName { get; set; }

        public double Rating { get; set; }

        //navigation property
        public int CountryId { get; set; }

        //relationship
        public Country? Country { get; set; }


        // Navigation Properties
        public ICollection<Booking>? Bookings { get; set; }

        // Many-to-Many with HotelAdmin
        public ICollection<HotelAdmin>? HotelAdmins { get; set; }

    }
}
