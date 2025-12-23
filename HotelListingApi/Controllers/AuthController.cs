using HotelListingApi.Common;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.DTOs.ApplicationUserDtos;
using HotelListingApi.Interfaces;
using HotelListingApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TodoListapp.CustomActionFilters;

namespace HotelListingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ValidateModel]



    public class AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService, HotelListDbContext hotelListDbContext, IUserService UserService) : BaseApiController

    {

        // POST: api/auth/register
        [HttpPost("register")]
        [ValidateModel]
        public async Task<ActionResult<RegisteredUserDto>> Register([FromBody] RegisterUserDto registerDto)
        {
            var result = await UserService.RegisterAsync(registerDto);

            return ToActionResult(result);


        }



        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        [ValidateModel]

        public async Task<ActionResult<LoginUserResponseDto>> Login([FromBody] LoginUserDto loginDto)

        {
            var result = await UserService.LoginAsync(loginDto);

            return ToActionResult(result);



        }


    }
} 
