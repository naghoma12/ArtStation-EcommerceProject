using ArtStation.Core.Entities;
using ArtStation_Dashboard.ViewModels;
using AutoMapper;

namespace ArtStation_Dashboard.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreatedCategory, Category>().ReverseMap();
        }
    }
}
