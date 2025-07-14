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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var traderUsers = await _userManager.GetUsersInRoleAsync(Roles.Customer);

            var totalUsers = traderUsers.Count;
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = traderUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Image = u.Image,
                   IsActive=u.IsActive,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(users);
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
