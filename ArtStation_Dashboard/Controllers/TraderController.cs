using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Roles;
using ArtStation_Dashboard.Helper;
using ArtStation_Dashboard.ViewModels.User;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace ArtStation_Dashboard.Controllers
{
    public class TraderController : Controller
    {
        //private readonly UserHelper userHelper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public TraderController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork,
            RoleManager<AppRole> roleManager, IMapper mapper, IWebHostEnvironment environment)
        {
            
            _userManager = userManager;
            this.unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _mapper = mapper;
            _environment = environment;

        }
        //public async Task<IActionResult> Index()
        //{
        //    var users = await _userManager.Users.Select(u => new UserViewModel()
        //    {
        //        Id = u.Id,
        //        Photo = u.Photo,
        //        UserName = u.UserName,


        //        IsCompany = u.IsCompany,
        //        Roles = new List<string>()
        //    }).Where(u => u.IsCompany == false).ToListAsync();


        //    foreach (var user in users)
        //    {
        //        user.Roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
        //    }

        //    return View(users);
        //}

        public async Task<IActionResult> AddTrader()
        {

            TraderViewModel user = new TraderViewModel();
            //var roles = await _roleManager.Roles.ToListAsync();
            //user.Roles = roles;
            user.Cities = await unitOfWork.Repository<Shipping>().GetAllAsync();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddTrader(TraderViewModel addUser)
        {
            try
            {
                if (addUser.PhotoFile != null)
                {
                    addUser.Photo = await FileSettings.UploadFile(addUser.PhotoFile,"Users",_environment.WebRootPath);
                }

                // Create a new AppUser object
                var user = new AppUser()
                {
                    UserName = addUser.UserName,
                    FullName=addUser.DispalyName,
                    Email = addUser.Email,
                    PhoneNumber = addUser.PhoneNumber,
                    Nationality= addUser.Nationality,
                    Country=addUser.City,


                    Image = $"Uploads/Users/{addUser.Photo}"
                };

                // Create the user in the system
                var createUserResult = await _userManager.CreateAsync(user, addUser.Password);

                if (createUserResult.Succeeded)
                {
                    // Add the role to the user after the user has been successfully created
                 
                        var addRoleResult = await _userManager.AddToRoleAsync(user, Roles.Trader);

                        if (!addRoleResult.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, "Failed to add role to the user.");
                            return View(addUser);
                        }
                    
                    

                    return RedirectToAction("Index","Home");
                }
                else
                {
                    // If user creation failed, add the errors to the ModelState
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message.ToString() ?? ex.Message.ToString());
            }

    }
}
