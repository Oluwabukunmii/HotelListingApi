using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.HotelDtos
{
    public class CreateHotelDto
    {
     
        [Required]
        [MinLength(2, ErrorMessage = "Code has to be a minimum length of 2 characters")]
        [MaxLength(20, ErrorMessage = "Code has to be a maximum length of 20 characters")]

        public string Name { get; set; }


        [Required]
        [MinLength(2, ErrorMessage = "Code has to be a minimum length of 1 characters")]
        [MaxLength(100, ErrorMessage = "Code has to be a maximum length of 100 characters")]

        public string Address { get; set; }

        [Required]
        public string CountryName { get; set; }

        [Required]
        public double Rating { get; set; }

        //navigation property
        public int CountryId { get; set; }

       
    }
}


