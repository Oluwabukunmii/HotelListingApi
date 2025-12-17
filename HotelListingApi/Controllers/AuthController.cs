using HotelListingApi.Common;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.DTOs.ApplicationUserDtos;
using HotelListingApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TodoListapp.CustomActionFilters;

namespace HotelListingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenService tokenService;
        private readonly HotelListDbContext hotelListDbContext;

        public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService, HotelListDbContext hotelListDbContext)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.hotelListDbContext = hotelListDbContext;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        [ValidateModel]
        public async Task<ActionResult<string>> Register([FromBody] RegisterUserDto registerDto)
        {
            var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return ToActionResult(Result.Failure(
                    new Error(ErrorTypes.Conflict, "User with this email already exists.")
                ));

            //creating identityuser

            var identityUser = new IdentityUser
            {

                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            var result = await userManager.CreateAsync(identityUser, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return ToActionResult(Result.Failure(
                    new Error(ErrorTypes.Validation, errors)
                ));
            }

            await userManager.AddToRoleAsync(identityUser, "User");


            // Add matching record in your custom Users table
            var applicationUser = new ApplicationUser
            {
                ApplicationUserId = identityUser.Id,
                Email = registerDto.Email,
                FullName = registerDto.fullName,
            };

            hotelListDbContext.ApplicationUser.Add(applicationUser);

            await hotelListDbContext.SaveChangesAsync();

            return ToActionResult(Result.Success("User successfully created"));


        }
        //Hotel admin endpoint
        [HttpPost("register-hotel-admin")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> RegisterHotelAdmin(RegisterUserDto dto)
        {
            var identityUser = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(identityUser, "HotelAdmin");

            hotelListDbContext.ApplicationUser.Add(new ApplicationUser
            {
                ApplicationUserId = identityUser.Id,
                Email = dto.Email,
                FullName = dto.fullName
            });

            await hotelListDbContext.SaveChangesAsync();
            return Ok("Hotel Admin registered successfully");
        }


        // POST: api/auth/login
        [HttpPost("login")]
        [ValidateModel]

        public async Task<ActionResult<LoginUserResponseDto>> Login([FromBody] LoginUserDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return ToActionResult(Result.Failure(
                    new Error(ErrorTypes.BadRequest, "Invalid email or password.")
                ));

            var passwordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
                return ToActionResult(Result.Failure(
                    new Error(ErrorTypes.BadRequest, "Invalid email or password.")
                ));

            var token = await tokenService.CreateJWTToken(user);

            var response = new LoginUserResponseDto
            {
                JwtToken = token
            };

            return ToActionResult(Result<LoginUserResponseDto>.Success(response));


        }
    }
}
 