using AutoMapper;
using HotelListingApi.Domain.Models;
using HotelListingApi.DTOs.BookingsDtos;
using HotelListingApi.DTOs.CountryDtos;
using HotelListingApi.DTOs.HotelDtos;

namespace HotelListingApi.AutoMapper
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<CreateCountryDto, Country>().ReverseMap();


            CreateMap<CreateHotelDto, Hotel>().ReverseMap();

            CreateMap<UpdateCountryDto, Country>().ReverseMap();

            CreateMap< UpdateHotelDto ,Hotel>().ReverseMap();

            CreateMap<UpdateHotelDto, Hotel>().ReverseMap();

            CreateMap<HotelDto, Hotel>().ReverseMap();

            CreateMap<CountryDto, Country>().ReverseMap();

            CreateMap<Booking, BookingDto>().ReverseMap ();

            CreateMap<CreateBookingDto, Booking>().ReverseMap();  
            CreateMap<UpdateBookingDto, Booking>().ReverseMap ();








        }
    }
}





