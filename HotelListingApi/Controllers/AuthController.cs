using HotelListingApi.Common;
using HotelListingApi.DTOs.ApplicationUserDtos;
using HotelListingApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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

        public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        [ValidateModel]
        public async Task<ActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return ToActionResult(Result.Failure(
                    new Error(ErrorTypes.Conflict, "User with this email already exists.")
                ));

            var user = new IdentityUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return ToActionResult(Result.Failure(
                    new Error(ErrorTypes.Validation, errors)
                ));
            }

            return ToActionResult(Result.Success());
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
 