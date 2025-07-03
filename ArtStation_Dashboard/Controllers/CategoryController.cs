using ArtStation.Core.Entities;
using ArtStation.Core;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ArtStation_Dashboard.ViewModels;
using ArtStation_Dashboard.Helper;
using Microsoft.AspNetCore.Localization;
using ArtStation_Dashboard.Resource;

namespace ArtStation_Dashboard.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper
            , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _environment = webHostEnvironment;

        }
        // Get All Categories --GET
        public async Task<IActionResult> Index(int page = 1 , int pageSize = 8)
        {
            try
            {
                string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName;
                ViewData["Language"] = language;
                var List = await _unitOfWork.Repository<Category>().GetAllAsync(page , pageSize);
                return View(List);
            }
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }

        // Get Category By Id --GET
        public async Task<IActionResult> Details(int id)
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName;
            ViewData["Language"] = language;
            var item = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (item == null) return NotFound();
            var itemMapped = _mapper.Map<Category, CategoryVM>(item);
            return View(itemMapped);
        }

        //Open the form --Get
        [HttpGet]
        public IActionResult Create()
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName;
            ViewData["Language"] = language;
            return View();
        }
        //Create New Category --post  Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatedCategory category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (category.PhotoFile != null)
                    {
                        category.Image = Guid.NewGuid().ToString() + Path.GetExtension(category.PhotoFile.FileName);
                        await FileSettings.UploadFile(category.PhotoFile, "Categories", _environment.WebRootPath);
                    }
                    var CatMapped = _mapper.Map<CreatedCategory, Category>(category);
                    _unitOfWork.Repository<Category>().Add(CatMapped);
                    var count = await _unitOfWork.Complet();
                    string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
                    if (count > 0)
                    {
                        TempData["message"] = ("تم إضافة تفاصيل القسم بنجاح","Category details added successfully").Localize(language); 
                        return RedirectToAction(nameof(Index));
                    }
                    else
                        TempData["message"] = ("فشلت عملية الإضافه", "The Addion operration failed").Localize(language);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException?.Message.ToString() ?? ex.Message.ToString());
                }
            }

            return View(category);
        }


        //Open the form of edit --get
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return await Details(id);
        }

        //Edit category --post Category/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryVM categoryVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (categoryVM.PhotoFile != null)
                    {
                        FileSettings.DeleteFile("Categories",categoryVM.Image,_environment.WebRootPath);
                        categoryVM.Image = Guid.NewGuid().ToString() + Path.GetExtension(categoryVM.PhotoFile.FileName);
                        categoryVM.Image = await FileSettings.UploadFile(categoryVM.PhotoFile, "Categories", _environment.WebRootPath);
                    }
                    var catMapped = _mapper.Map<CategoryVM, Category>(categoryVM);
                   // catMapped.FilePath = Path.Combine(_environment.ContentRootPath, "wwwroot\\Uploads\\category", catMapped.PhotoURL);
                    _unitOfWork.Repository<Category>().Update(catMapped);
                    var count = await _unitOfWork.Complet();
                    if (count > 0)
                    {
                        TempData["Message"] = $"تم تعديل تفاصيل القسم بنجاح";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException?.Message.ToString() ?? ex.Message.ToString());
                }
            }
            return View(categoryVM);
        }


        //Open form of delete --Get Category/delete/id
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            ViewData["Language"] = language;
            return await Details(id);
        }

        //Delete category --post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CategoryVM categoryVM)
        {
             string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            try
            {
                var  existCategory = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
                FileSettings.DeleteFile("Categories", existCategory.Image, _environment.WebRootPath);
                existCategory.IsDeleted = true;
                existCategory.IsActive = false;
                _unitOfWork.Repository<Category>().Update(existCategory);
                var count = await _unitOfWork.Complet();
                if (count > 0)
                {
                    TempData["Message"] = ("تم حذف تفاصيل القسم بنجاح" , "Category deleted successfully").Localize(language);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                TempData["Message"] = ("فشلت عملية الحذف","The deletion operation failed").Localize(language);
            }
               return View(categoryVM);
        }

    }
}

