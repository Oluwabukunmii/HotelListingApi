using HotelListingApi.DTOs.HotelDtos;

namespace HotelListingApi.DTOs.CountryDtos
{
    public class CountryV2Dto
    {
        public int CountryId { get; set; }

        public string Name { get; set; }


        public List<HotelDto> Hotels { get; set; } = new();

    }
}
