using HotelListingApi.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelListingApi.Interfaces
{

    public interface ITokenService
    {
        Task<string> CreateJWTToken(IdentityUser user);
    }


}
