using ArtStation.Core.Entities;
using ArtStation.Core;
using ArtStation_Dashboard.Helper;
using ArtStation_Dashboard.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Roles;
using ArtStation.Repository;
using ArtStation_Dashboard.Resource;
using ArtStation_Dashboard.ViewModels.User;

namespace ArtStation_Dashboard.Controllers
{
    public class BannerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public BannerController(IUnitOfWork unitOfWork, IMapper mapper
            , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _environment = webHostEnvironment;

        }

        //Get : Get All Banners

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            try
            {
              
                var banners = await _unitOfWork.Repository<Banner>().GetAllAsync(page, pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = banners.TotalPages;
                return View(banners);
            }
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }

        //Get : Get Banner Details
        public async Task<IActionResult> Details(int id)
        {
            var banner = await _unitOfWork.Repository<Banner>().GetByIdAsync(id);

            return View(banner);
        }


        //Get : Add Trader
        public async Task<IActionResult> AddBanner()
        {
            return View();
        }

        //Post : Add Trader
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBanner(BannerVM bannerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(bannerVM);
            }

            try
            {
              
                
                var banner=_mapper.Map<Banner>(bannerVM);

                TempData["SuccessMessage"] = ViewMessages.AddBannerSuccessfully;
                return RedirectToAction("Index", "Banner");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                return View(bannerVM);
            }
        }



        //Get : Edit Trader
        //public async Task<IActionResult> Edit(int id)
        //{

        //    var traderVM = await _userHelper.Edit(id);

        //    traderVM.Cities = await unitOfWork.Repository<Shipping>().GetAllAsync();
        //    ViewData["ActionOne"] = "EditVendor";
        //    return View(traderVM);

        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(TraderViewModel traderVM)
        //{
        //    try
        //    {
        //        var trader = await _userManager.FindByIdAsync(traderVM.Id.ToString());

        //        if (trader == null)
        //        {
        //            //var result=ModelState.AddModelError("", "المستخدم غير موجود.");
        //            //return result;
        //        }


        //        if (traderVM.PhotoFile != null)
        //        {
        //            if (!string.IsNullOrEmpty(trader.Image))
        //            {
        //                FileSettings.DeleteFile("Users", trader.Image, _environment.WebRootPath);
        //            }

        //            trader.Image = await FileSettings.UploadFile(traderVM.PhotoFile, "Users", _environment.WebRootPath);
        //        }


        //        trader.UserName = traderVM.UserName;
        //        trader.Email = traderVM.Email;
        //        trader.PhoneNumber = traderVM.PhoneNumber;
        //        trader.FullName = traderVM.DispalyName;
        //        trader.Country = traderVM.City;
        //        trader.Nationality = traderVM.Nationality;

        //        var result = await _userManager.UpdateAsync(trader);

        //        if (result.Succeeded)
        //        {
        //            TempData["SuccessMessage"] = ViewMessages.AddTraderSucessfully;
        //            return RedirectToAction("Index");
        //        }


        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
        //    }


        //    traderVM.Cities = await _unitOfWork.Repository<Shipping>().GetAllAsync();
        //    return View(traderVM);
        //}





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            var banner = await _unitOfWork.Repository<Banner>().GetByIdAsync(id);
            if (banner == null) return NotFound();

            banner.IsActive = isActive;
            _unitOfWork.Repository<Banner>().Update(banner); 

            var count = await _unitOfWork.Complet(); 
            if (count > 0)
            {
                return Ok();
            }
            return BadRequest("Failed to update the banner status.");
        }
    }
}
