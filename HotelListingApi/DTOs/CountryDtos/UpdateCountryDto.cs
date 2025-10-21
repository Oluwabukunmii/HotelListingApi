using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.CountryDtos
{
    public class UpdateCountryDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "Code has to be a minimum length of 2 characters")]
        [MaxLength(20, ErrorMessage = "Code has to be a maximum length of 20 characters")]

        public string Name { get; set; }
        public string ShortName { get; set; }

    }
}
