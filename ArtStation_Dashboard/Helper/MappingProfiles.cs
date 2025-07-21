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
            CreateMap<ProductCreation, Product>().ReverseMap();


        }
    }
}
