using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Cart;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Order;
using ArtStation.Dtos;
using ArtStation.Dtos.AuthDtos;
using ArtStation.Dtos.CartDtos;
using ArtStation.Dtos.Order;
using ArtStation.Dtos.UserDtos;
using ArtStation.DTOS;
using AutoMapper;
using System.Globalization;

namespace ArtStation.Helper
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryWithProducts>().ReverseMap();
            CreateMap<Shipping,ShippingDto>().ReverseMap();
            CreateMap<Address,AddressDtoUseId>().ReverseMap();
            CreateMap<SimpleProduct, Product>().ReverseMap();
            CreateMap<AppUser, UserProfileDto>()
     .ForMember(dest => dest.Fname, opt => opt.MapFrom(src =>
         string.IsNullOrWhiteSpace(src.FullName)
             ? null
             : src.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()))
     .ForMember(dest => dest.LName, opt => opt.MapFrom(src =>
         string.IsNullOrWhiteSpace(src.FullName)
             ? null
             : src.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault()))
     .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.GetLocalizedDisplayName()))
     .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src =>
         src.BirthDay.HasValue ? src.BirthDay.Value.ToString("yyyy-MM-dd") : null))
     .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
     .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
     .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Image))
     .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality))
     .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));


            CreateMap<UpdateUserProfileDto, AppUser>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
        $"{src.Fname} {src.LName}".Trim()))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
     .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.PhoneNumber))
    .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src =>
        Common.ConvertBirthday(src.Birthday)))
    .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src =>
        string.IsNullOrWhiteSpace(src.Nationality) ? null : src.Nationality))
    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
    EnumHelper.ParseGender(src.Gender)));


            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<CartItem, CartItemDto>().ReverseMap();
            CreateMap<Address, DeliveryAddress>()
                 .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Shipping.City))
                 .ReverseMap();
            CreateMap<CartItem, CartItemReturnDto>().ReverseMap();
            //CreateMap<Cart, CartReturnDto>().ReverseMap();

            CreateMap<Banner, BannerDto>()
     .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => $"Uploads/Banners/{src.ImageUrl}"))
     .ForMember(dest => dest.Order, opt => opt.MapFrom(src =>src.OrderBanner))
     .ReverseMap();

            CreateMap<Order, OrderReturnDto>()
                 .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id+1000))
                 .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("MMM dd, yyyy",  CultureInfo.CurrentCulture)))
                  .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status))
                   .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
                   .ForMember(dest => dest.ShippingCost, opt => opt.MapFrom(src => src.Address.Shipping.Cost))
                    .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.SubTotal+src.Address.Shipping.Cost))
                 .ReverseMap();


        }
    }
}
