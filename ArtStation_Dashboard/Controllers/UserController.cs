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
        public async Task<IActionResult> Index(string search, bool? statusFilter, int page = 1, int pageSize = 5)
        {
            try
            {
                var customers = await _userManager.GetUsersInRoleAsync(Roles.Customer);

                if (statusFilter != null)
                    customers = customers.Where(i => i.IsActive == statusFilter).ToList();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim().ToLower();
                    customers = customers.Where(i =>
                        (!string.IsNullOrEmpty(i.FullName) && i.FullName.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(i.PhoneNumber) && i.PhoneNumber.ToLower().Contains(search)) 
                      
                    ).ToList();
                }

                customers = customers.Where(i => !i.IsDeleted).ToList();

                var totalUsers = customers.Count;

                var users = customers
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

                var model = new PagedResult<UserViewModel>
                {
                    Items = users,
                    TotalItems = totalUsers,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages= (int)Math.Ceiling((double)totalUsers / pageSize),

                };

                ViewBag.StatusFilter = statusFilter;
                ViewBag.Search = search;

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_UserTablePartial", model);

                return View(model);
               
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }

        }

       
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
                    TempData["SuccessMessage"] = ViewMessages.EditUserSucessfully;
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
        public async Task<IActionResult> ToggleActive(int id, bool isActive, string reason)
        {
            try
            {
                var trader = await _userManager.FindByIdAsync(id.ToString());
                if (trader == null)
                {
                    return NotFound("المستخدم غير موجود");
                }

                trader.IsActive = isActive;

                if (!isActive && !string.IsNullOrWhiteSpace(reason))
                {
                    trader.InActiveMessage = reason;
                    trader.DeactivatedAt = DateTime.UtcNow;
                }
                else if (isActive)
                {
                    // Reset message and time if re-activated
                    trader.InActiveMessage = null;
                    trader.DeactivatedAt = null;
                }

                var result = await _userManager.UpdateAsync(trader);

                if (result.Succeeded)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return BadRequest($"فشل في التحديث: {errors}");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"حدث خطأ غير متوقع: {ex.Message}");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    return NotFound();

                user.IsDeleted = true;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    // Collect error messages and return as BadRequest
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return BadRequest(new { message = errors });
                }

                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                // Log exception here if you have logging configured
                // _logger.LogError(ex, "Error deleting user with ID {Id}", id);

                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }



    }
}
