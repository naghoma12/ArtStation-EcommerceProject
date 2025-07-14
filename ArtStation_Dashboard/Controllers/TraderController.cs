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
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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
        private readonly UserHelper _userHelper;

        public TraderController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork,
            RoleManager<AppRole> roleManager, IMapper mapper, IWebHostEnvironment environment,
            UserHelper userHelper)
        {

            _userManager = userManager;
            this.unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _mapper = mapper;
            _environment = environment;
            _userHelper = userHelper;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
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
                    IsActive=u.IsActive,
                    PhoneNumber = u.PhoneNumber
                }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        //Get : Add Trader
        public async Task<IActionResult> AddTrader()
        {

            TraderViewModel user = new TraderViewModel();

            user.Cities = await unitOfWork.Repository<Shipping>().GetAllAsync();
            return View(user);
        }

        //Post : Add Trader
        [HttpPost]
        [ValidateAntiForgeryToken]
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


        //Get : Get Trader Details
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var mappesuser = _mapper.Map<AppUser, TraderViewModel>(user);
            return View(mappesuser);
        }

        //Get : Edit Trader
        public async Task<IActionResult> Edit(int id)
        {

            var traderVM = await _userHelper.Edit(id);

            traderVM.Cities = await unitOfWork.Repository<Shipping>().GetAllAsync();
            ViewData["ActionOne"] = "EditVendor";
            return View(traderVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TraderViewModel traderVM)
        {
            try
            {
                var trader = await _userManager.FindByIdAsync(traderVM.Id.ToString());

                if (trader == null)
                {
                    //var result=ModelState.AddModelError("", "المستخدم غير موجود.");
                    //return result;
                }


                if (traderVM.PhotoFile != null)
                {
                    if (!string.IsNullOrEmpty(trader.Image))
                    {
                        FileSettings.DeleteFile("Users", trader.Image, _environment.WebRootPath);
                    }

                    trader.Image = await FileSettings.UploadFile(traderVM.PhotoFile, "Users", _environment.WebRootPath);
                }


                trader.UserName = traderVM.UserName;
                trader.Email = traderVM.Email;
                trader.PhoneNumber = traderVM.PhoneNumber;
                trader.FullName = traderVM.DispalyName;
                trader.Country = traderVM.City;
                trader.Nationality = traderVM.Nationality;

                var result = await _userManager.UpdateAsync(trader);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = ViewMessages.AddTraderSucessfully;
                    return RedirectToAction("Index");
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
            }


            traderVM.Cities = await unitOfWork.Repository<Shipping>().GetAllAsync();
            return View(traderVM);
        }




        public async Task<IActionResult> ChangePassword(int id)
        {
            var trader = await _userManager.FindByIdAsync(id.ToString());
            ViewBag.TraderName = trader.UserName;
            ViewBag.TraderId = trader.Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM passwordVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var trader = await _userManager.FindByIdAsync(passwordVM.TraderId.ToString());
                    if (trader == null)
                    {
                        ModelState.AddModelError("", "المستخدم غير موجود.");
                        return View(passwordVM);
                    }

                    var token = await _userManager.GeneratePasswordResetTokenAsync(trader);
                    var result = await _userManager.ResetPasswordAsync(trader, token, passwordVM.NewPassword);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = ViewMessages.ChangePasswordSuccessfully;
                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                }
            }

            var traderFallback = await _userManager.FindByIdAsync(passwordVM.TraderId.ToString());
            if (traderFallback != null)
            {
                ViewBag.TraderName = traderFallback.UserName;
                ViewBag.TraderId = traderFallback.Id;
            }

            return View(passwordVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            var trader = await _userManager.FindByIdAsync(id.ToString());
            if (trader == null) return NotFound();

            trader.IsActive = isActive;
            var result = await _userManager.UpdateAsync(trader);

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }


    }
}
