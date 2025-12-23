using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.ApplicationUserDtos
{
    public class RegisterUserDto : IValidatableObject
    {
        [Required]

        public string fullName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]

        public string Email { get; set; }

        [Required]

        public string Password { get; set; }

        public string? Role { get; set; } = "User";

        public int? AssociatedHotelId { get; set; }

        //custom validation attribute
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == "HotelAdmin" && AssociatedHotelId.GetValueOrDefault() < 1)
            {
                yield return new ValidationResult(
                    "Please provide a valid Hotel Id",
                    [nameof(AssociatedHotelId)]);
            }


        }
    }
}