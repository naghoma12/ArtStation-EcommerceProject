using ArtStation.Core.Entities.Identity;
using ArtStation_Dashboard.ViewModels.User;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace ArtStation_Dashboard.Helper
{
    public class UserHelper
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;

        public UserHelper(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user;
        }
        public async Task<TraderViewModel> Edit(int id)
        {
            var user = await GetUserByIdAsync(id);
            var mappesuser = _mapper.Map<AppUser, TraderViewModel>(user);
            return mappesuser;

        }
    }
}
