namespace HotelListingApi.Domain.Models
{
    public class HotelAdmin
    {
        public int Id { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        // Many-to-Many with Hotel
        public ICollection<Hotel>? Hotels { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
        
        public string? ApplicationUserId { get; set; }// links to IdentityUser


    }
}
 