using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.ApplicationUserDtos
{
    public class RegisteredUserDto 
    {

        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;



    }

}

    