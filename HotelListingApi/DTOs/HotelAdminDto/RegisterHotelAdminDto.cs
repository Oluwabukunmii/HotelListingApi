using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.HotelAdminDto
{
    public class RegisterHotelAdminDto
    {

        [Required]
        public string UserName { get; set; }

        [Required]

        public string Email { get; set; }

        [Required]

        public string Password { get; set; }

        public string Role { get; set; } = "HotelAdmin";

        //Relationship

        public int? AssociatedHotelId { get; set; }

        //Navigation Property


    }
}
