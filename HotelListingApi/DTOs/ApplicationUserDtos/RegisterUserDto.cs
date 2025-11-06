using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.ApplicationUserDtos
{
    public class RegisterUserDto
    {
        [Required]

        public string fullName { get; set; }

        [Required]  
        public string UserName { get; set; }

        [Required]

        public string Email { get; set; }

        [Required]

        public string Password { get; set; }
    }
}
