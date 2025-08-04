using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;
using ArtStation.Core.Roles;
using ArtStation.Core;
using ArtStation_Dashboard.Helper;
using ArtStation_Dashboard.Resource;
using ArtStation_Dashboard.ViewModels.User;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ArtStation_Dashboard.ViewModels;

namespace ArtStation_Dashboard.Controllers
{
    public class UserController : Controller
    {
        //private readonly UserHelper userHelper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly UserHelper _userHelper;

        public UserController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork,
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
        public async Task<IActionResult> Index(bool? statusFilter, int page = 1, int pageSize = 5)
        {
            try
            {
                var Users = await _userManager.GetUsersInRoleAsync(Roles.Customer);

                var totalUsers = Users.Count;
                var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
                ViewBag.StatusFilter = statusFilter;
                var users = new List<UserViewModel>();
                if (statusFilter == null)
                {
                    users = Users.Where(i=>i.IsDeleted==false)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Image = u.Image,
                    IsActive = u.IsActive,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                }).ToList();

                }
                else
                {
                    users = Users
                        .Where(i => i.IsActive == statusFilter)
                     .Where(i => i.IsDeleted == false)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .Select(u => new UserViewModel
               {
                   Id = u.Id,
                   Image = u.Image,
                   IsActive = u.IsActive,
                   FullName = u.FullName,
                   Email = u.Email,
                   PhoneNumber = u.PhoneNumber
               }).ToList();

               
                }

                var customers = new PagedResult<UserViewModel>
                {
                    Items = users,
                    TotalItems = totalUsers,
                    PageNumber = page,
                    PageSize = pageSize,

                };

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_TraderTablePartial", customers);
                }

                return View(customers);
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }

        }

        //public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        //{
        //    var traderUsers = await _userManager.GetUsersInRoleAsync(Roles.Customer);

        //    var totalUsers = traderUsers.Count;
        //    var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

        //    var users = traderUsers
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(u => new UserViewModel
        //        {
        //            Id = u.Id,
        //            Image = u.Image,
        //           IsActive=u.IsActive,
        //            FullName = u.FullName,
        //            Email = u.Email,
        //            PhoneNumber = u.PhoneNumber
        //        }).ToList();

        //    ViewBag.CurrentPage = page;
        //    ViewBag.TotalPages = totalPages;

        //    return View(users);
        //}



        //Get : Get User Details
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var mappesuser = _mapper.Map<AppUser,UserViewModel>(user);
            return View(mappesuser);
        }

        //Get : Edit User
        public async Task<IActionResult> Edit(int id)
        {

            var userVM = await _userHelper.EditUser(id);

            userVM.Cities = await unitOfWork.Repository<Shipping>().GetAllAsync();
           
            return View(userVM);

        }

        //Post : Edit User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel userVM)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userVM.Id.ToString());

                if (user == null)
                {
                   ModelState.AddModelError("", "المستخدم غير موجود.");
                   
                }


                //if (userVM.ImageFile != null)
                //{
                //    if (!string.IsNullOrEmpty(user.Image))
                //    {
                //        FileSettings.DeleteFile("Users", user.Image, _environment.WebRootPath);
                //    }

                //    user.Image = await FileSettings.UploadFile(userVM.ImageFile, "Users", _environment.WebRootPath);
                //}


                user.FullName = userVM.FullName;
                user.Email = userVM.Email;
                user.PhoneNumber = userVM.PhoneNumber;
                user.Gender = EnumHelper.ParseGender(userVM.Gender);
                user.Country = userVM.Country;
                user.Nationality = userVM.Nationality;
                user.BirthDay = userVM.BirthDay;
                var result = await _userManager.UpdateAsync(user);

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


            userVM.Cities = await unitOfWork.Repository<Shipping>().GetAllAsync();
            return View(userVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return Json(new { success = false, message = "المستخدم غير موجود." });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Description).ToList();
                return Json(new { success = false, message = string.Join(" - ", errorMessages) });
            }
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
