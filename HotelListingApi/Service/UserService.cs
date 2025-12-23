using HotelListingApi.Common;
using HotelListingApi.Domain.Models;
using HotelListingApi.Domain;
using Microsoft.AspNetCore.Identity;
using HotelListingApi.DTOs.ApplicationUserDtos;
using HotelListingApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Service
{
    public class UserService: IUserService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenService tokenService;
        private readonly HotelListDbContext hotelListDbContext;

        public UserService(UserManager<IdentityUser> userManager, ITokenService tokenService, HotelListDbContext hotelListDbContext)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.hotelListDbContext = hotelListDbContext;
        }

       
        public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerDto)
        {

            var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)

                return Result<RegisteredUserDto>.Failure(
                    new Error(ErrorTypes.Conflict, "User with this email already exists.")
                );

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
                return Result<RegisteredUserDto>.Failure(
                    new Error(ErrorTypes.Validation, errors)
                );
            }

            await userManager.AddToRoleAsync(identityUser, "User");


            // Add matching record in your custom Users table
            var applicationUser = new ApplicationUser
            {
                ApplicationUserId = identityUser.Id,
                Email = registerDto.Email,
                FullName = registerDto.fullName,
            };

            // If Hotel Admin, add to HotelAdmins table

            if (registerDto.Role == "HotelAdmin")
            {
                if (!registerDto.AssociatedHotelId.HasValue)
                    throw new Exception("Hotel Admin must be assigned to a hotel");

                var hotel = await hotelListDbContext.Hotels
                                                    .FirstOrDefaultAsync(h => h.Id == registerDto.AssociatedHotelId.Value);

                if (hotel == null)
                    throw new Exception("Hotel not found");

                var hotelAdmin = new HotelAdmin
                {
                    ApplicationUserId = identityUser.Id,   // You are assigning a logged-in user (IdentityUser) to be a hotel admin in your system
                    AdminName = identityUser.UserName ?? "",
                    Email = identityUser.Email ?? "",
                    AssignedDate = DateTime.UtcNow,
                    Hotels = new List<Hotel> { hotel }
                };

                hotelListDbContext.ApplicationUser.Add(applicationUser);
                if (registerDto.Role == "HotelAdmin")
                {
                    hotelListDbContext.HotelAdmins.Add(hotelAdmin);
                }

                await hotelListDbContext.SaveChangesAsync();


            }

            //return ToActionResult(Result.Success("User successfully created"));

            var registeredUser = new RegisteredUserDto
            {
                Email = identityUser.Email,
                fullName = registerDto.fullName ?? "",
                Id = identityUser.Id,
                Role = registerDto.Role,
            };


            // Optional: Send confirmation Email
            return Result<RegisteredUserDto>.Success(registeredUser);
        }


        public async Task<Result<LoginUserResponseDto>> LoginAsync(LoginUserDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return Result<LoginUserResponseDto>.Failure(
                    new Error(ErrorTypes.BadRequest, "Invalid email or password.")
                );

            var passwordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
                return Result<LoginUserResponseDto>.Failure(
                                    new Error(ErrorTypes.BadRequest, "Invalid email or password."));

            var token = await tokenService.CreateJWTToken(user);

            var response = new LoginUserResponseDto
            {
                JwtToken = token
            };

            return Result<LoginUserResponseDto>.Success(response);

        }

    }
}

