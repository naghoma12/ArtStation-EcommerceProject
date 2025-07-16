using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Identity;
using ArtStation_Dashboard.ViewModels;
using ArtStation_Dashboard.ViewModels.User;
using AutoMapper;

namespace ArtStation_Dashboard.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreatedCategory, Category>().ReverseMap();
            CreateMap<CategoryVM, Category>().ReverseMap();
            CreateMap<AppUser,TraderViewModel>().ForMember(dest=>dest.Photo,src=>src.MapFrom(opt=>opt.Image))
                .ForMember(dest => dest.DispalyName, src => src.MapFrom(opt => opt.FullName))
                .ForMember(dest => dest.City, src => src.MapFrom(opt => opt.Country))
                .ForMember(dest => dest.Photo, src => src.MapFrom(opt => opt.Image))
                .ReverseMap();
            CreateMap<AppUser, UserViewModel>()
                .ForMember(dest => dest.Image, src => src.MapFrom(opt => opt.Image))
              .ForMember(dest => dest.FullName, src => src.MapFrom(opt => opt.FullName))
              .ForMember(dest => dest.Country, src => src.MapFrom(opt => opt.Country))
              //.ForMember(dest => dest.Image, src => src.MapFrom(opt => opt.Image))
              .ReverseMap();
            CreateMap<ProductCreation, Product>().ReverseMap();

        }
    }
}
