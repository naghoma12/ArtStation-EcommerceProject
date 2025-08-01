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


               
                if (bannerVM.Photo != null)
                {
                    bannerVM.ImageUrl = await FileSettings.UploadFile(bannerVM.Photo, "Banners", _environment.WebRootPath);
                }

                var banner =_mapper.Map<Banner>(bannerVM);
                _unitOfWork.Repository<Banner>().Add(banner);
                await _unitOfWork.Complet();

                TempData["SuccessMessage"] = ViewMessages.AddBannerSuccessfully;
                return RedirectToAction("Index", "Banner");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                return View(bannerVM);
            }
        }



        //Get : Edit Banner
        public async Task<IActionResult> Edit(int id)
        {
            var banner = await _unitOfWork.Repository<Banner>().GetByIdAsync(id);
            var mappedbanner = _mapper.Map<BannerVM>(banner);
            return View(mappedbanner);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BannerVM bannerVM)
        {
            try
            {
                var banner = await _unitOfWork.Repository<Banner>().GetByIdAsync(bannerVM.Id.Value);

                if (bannerVM.Photo != null)
                {
                    if (!string.IsNullOrEmpty(banner.ImageUrl))
                    {
                        FileSettings.DeleteFile("Banners", banner.ImageUrl, _environment.WebRootPath);
                    }

                    bannerVM.ImageUrl = await FileSettings.UploadFile(bannerVM.Photo, "Banners", _environment.WebRootPath);
                }
                bannerVM.ImageUrl = banner.ImageUrl;
                _mapper.Map(bannerVM, banner);

                _unitOfWork.Repository<Banner>().Update(banner);
                await _unitOfWork.Complet();
                return RedirectToAction("Index");
            
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
            }


           
            return View(bannerVM);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            var banner = await _unitOfWork.Repository<Banner>().GetByIdAsync(id);
            if (banner == null) return NotFound();

            banner.IsActive = isActive;
            await _unitOfWork.Complet(); 

            return Ok();
        }


    }
}
