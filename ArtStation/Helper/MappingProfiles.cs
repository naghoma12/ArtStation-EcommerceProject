using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Identity;
using ArtStation.Dtos.AuthDtos;
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
        }
    }
}
