using System.Text;
using System.Threading.RateLimiting;
using HotelListing.Api.CachePolicies;
using HotelListingApi.AutoMapper;
using HotelListingApi.Common;
using HotelListingApi.Domain;
using HotelListingApi.Domain.Models;
using HotelListingApi.Interfaces;
using HotelListingApi.Service;
using HotelListingApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
                .AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddMemoryCache();


// ✅ Configure Swagger for JWT Authorization
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Listing API",
        Version = "v1"
    });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "Enter JWT Bearer token in the format: Bearer {your token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
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
                Scheme = "oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


// ✅ Add DbContexts
//builder.Services.AddDbContext<HotelListDbContext>(options =>
//  options.UseSqlServer(builder.Configuration.GetConnectionString("HotelListingConnectionString")));
var hotelConnectionString = builder.Configuration.GetConnectionString("HotelListingConnectionString");

builder.Services.AddDbContextPool<HotelListDbContext>(options =>
{
    options.UseSqlServer(hotelConnectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    });

    // Enable detailed logging and errors in development environment
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }

    // Disable tracking by default to improve read performance
   /* options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);*/

}, poolSize: 128);


builder.Services.AddDbContext<HotelListingAuthDbContext>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelListingAuthConnectionString")));


// ✅ Add Identity
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("HotelListing")
    .AddEntityFrameworkStores<HotelListingAuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});


// ✅ Configure Authentication (JWT)
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


// ✅ Add AutoMapper & Services
builder.Services.AddAutoMapper(typeof(AutomapperProfiles));
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IBookingService, BookingService>();

//Add outpt cache.

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy(CacheConstants.AuthenticatedUserCachingPolicy, builder =>
    {
        builder.AddPolicy<AuthenticatedUserCachingPolicy>()
        .SetCacheKeyPrefix(CacheConstants.AuthenticatedUserCachingPolicyTag);
    }, true);
});

//Add rate limiting.

builder.Services.AddRateLimiter(options =>
{
options.AddFixedWindowLimiter(RateLimitingConstants.FixedPolicy, opt =>
{
    opt.Window = TimeSpan.FromMinutes(1);
    opt.PermitLimit = 50;
    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    opt.QueueLimit = 5;
});

options.AddPolicy(RateLimitingConstants.PerUserPolicy, context =>
{
    var username = context.User?.Identity?.Name ?? "anonymous";

    return RateLimitPartition.GetSlidingWindowLimiter(username, _ => new SlidingWindowRateLimiterOptions
    {
        Window = TimeSpan.FromMinutes(1),
        PermitLimit = 50,
        SegmentsPerWindow = 6,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 3
    });
});

// Global rate limit by IP
options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
{
    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
    {
        Window = TimeSpan.FromMinutes(1),
        PermitLimit = 200,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 10
    });
});
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests",
            message = "Rate limit exceeded. Please try again later.",
            retryAfter = retryAfter.TotalSeconds
        }, cancellationToken: cancellationToken);
    };
});




// ✅ Build app ONCE
var app = builder.Build();


// ✅ Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Listing API v1");

        // Clean, compact Swagger UI
        options.DefaultModelsExpandDepth(-1); // hides schema panel
        options.DefaultModelExpandDepth(0);   // collapses model details
        options.DisplayRequestDuration();     // shows API response time

    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseOutputCache();

app.MapControllers();

app.Run();
