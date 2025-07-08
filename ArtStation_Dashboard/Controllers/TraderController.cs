using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Roles;
using ArtStation_Dashboard.Helper;
using ArtStation_Dashboard.Resource;
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
        public async Task<IActionResult> Index(int page = 1, int pageSize =5)
        {
            var traderUsers = await _userManager.GetUsersInRoleAsync(Roles.Trader);

            var totalUsers = traderUsers.Count;
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = traderUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new TraderViewModel
                {
                    Id = u.Id,
                    Photo = u.Image,
                    UserName = u.UserName,
                    DispalyName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }


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
            if (!ModelState.IsValid)
            {
                return View(addUser);
            }

            try
            {
                // Upload photo if exists
                if (addUser.PhotoFile != null)
                {
                    addUser.Photo = await FileSettings.UploadFile(addUser.PhotoFile, "Users", _environment.WebRootPath);
                }

                // Create AppUser object
                var user = new AppUser
                {
                    UserName = addUser.UserName,
                    FullName = addUser.DispalyName,
                    Email = addUser.Email,
                    PhoneNumber = addUser.PhoneNumber,
                    Nationality = addUser.Nationality,
                    Country = addUser.City,
                    Image = string.IsNullOrEmpty(addUser.Photo) ? null : $"Uploads/Users/{addUser.Photo}"
                };

                // Create the user
                var createUserResult = await _userManager.CreateAsync(user, addUser.Password);

                if (!createUserResult.Succeeded)
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(addUser);
                }

                // Assign role
                var addRoleResult = await _userManager.AddToRoleAsync(user, Roles.Trader);

                if (!addRoleResult.Succeeded)
                {
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    

                    return View(addUser);
                }
                TempData["SuccessMessage"] = ViewMessages.AddTraderSucessfully;
                return RedirectToAction("Index", "Trader");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                return View(addUser);
            }
        }

    }
}
