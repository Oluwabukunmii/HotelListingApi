using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using HotelListingApi.DTOs.ApplicationUserDtos;
using Microsoft.Win32;

namespace HotelListingApi.Interfaces
{
    public interface IUserService
    {
        Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerDto);
        Task<Result<LoginUserResponseDto>> LoginAsync(LoginUserDto loginDto);


    }
}
