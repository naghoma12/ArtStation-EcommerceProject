using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Cart;
using ArtStation.Core.Entities.Identity;
using ArtStation.Dtos;
using ArtStation.Dtos.AuthDtos;
using ArtStation.Dtos.CartDtos;
using ArtStation.Dtos.UserDtos;
using ArtStation.DTOS;
using AutoMapper;

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
            CreateMap<Address, DeliveryAddress>().ReverseMap();
            CreateMap<CartItem, CartItemReturnDto>().ReverseMap();
            //CreateMap<Cart, CartReturnDto>().ReverseMap();

            CreateMap<Banner, BannerDto>().ReverseMap();

           
        }
    }
}
