using ArtStation.Core.Entities;
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
        }
    }
}
