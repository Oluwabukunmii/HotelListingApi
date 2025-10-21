using Microsoft.AspNetCore.Identity;

namespace HotelListingApi.Interfaces
{

    public interface ITokenService
    {
        Task<string> CreateJWTToken(IdentityUser user);
    }


}
