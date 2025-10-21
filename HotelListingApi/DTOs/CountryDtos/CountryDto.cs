using HotelListingApi.DTOs.HotelDtos;

namespace HotelListingApi.DTOs.CountryDtos
{
    public class CountryDto
    {

        public int CountryId { get; set; }

        public string Name { get; set; }   

        public string ShortName { get; set; }

        public List<HotelDto> Hotels { get; set; } = new();




    }
}
