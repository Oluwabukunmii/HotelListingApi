using System.Configuration;
using System.Text;
using HotelListingApi.AutoMapper;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.Interfaces;
using HotelListingApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();

//configure swagger for authorization

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = " Todo List App Api", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "Enter JWT Bearer token in the format: Bearer {your token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,


    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement

   {
          {
        new OpenApiSecurityScheme
        {
             Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
          },

             Scheme ="Oauth2",
             Name = JwtBearerDefaults.AuthenticationScheme,
             In = ParameterLocation.Header

          },

        new List<string>()
        }
   });

}
);

//Add dbcontext


builder.Services.AddDbContext<HotelListDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelListingConnectionString")));

builder.Services.AddDbContext<HotelListingAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelListingAuthConnectionString")));





builder.Services.AddIdentityCore<IdentityUser>()                       //Add identity core
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("HotelListing")
    .AddEntityFrameworkStores<HotelListingAuthDbContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>                       //Add identity option
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});



// Configure Authentication - JWT Bearer

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)   
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });





builder.Services.AddAutoMapper(typeof(AutomapperProfiles));                                 //Add automapperprofiles
builder.Services.AddScoped<IHotelService, HotelService>();                                 //Add Services
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ITokenService, TokenService>();




//builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

//app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();


app.UseAuthorization();

app.MapControllers();

app.Run();
