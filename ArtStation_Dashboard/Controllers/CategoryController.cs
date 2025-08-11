using ArtStation.Core.Entities;
using ArtStation.Core;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ArtStation_Dashboard.ViewModels;
using ArtStation_Dashboard.Helper;
using Microsoft.AspNetCore.Localization;
using ArtStation_Dashboard.Resource;
using Microsoft.AspNetCore.Authorization;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Helper;
using System.Linq.Expressions;

namespace ArtStation_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper
            , IWebHostEnvironment webHostEnvironment
            , ICategoryRepository categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _environment = webHostEnvironment;
           _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> FilterCategories(string searchText, int page = 1, int pageSize = 5)
        {
            try
            {
                string language = HttpContext.Features.Get<IRequestCultureFeature>()?
                    .RequestCulture.Culture.TwoLetterISOLanguageName;
                ViewData["Language"] = language;

                Expression<Func<Category, bool>> filter = null;

                if (!string.IsNullOrEmpty(searchText))
                {
                    filter = c => c.NameAR.Contains(searchText) || c.NameEN.ToLower().Contains(searchText.ToLower()) && c.IsActive && !c.IsDeleted;
                }
                filter = filter ?? (c => c.IsActive && !c.IsDeleted);
                var result = await _categoryRepository.GetFilteredAsync(filter, page, pageSize);

                return PartialView("_Category",result);
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }
        // Get All Categories --GET
        public IActionResult Index()
        {
            return View();  
        }

        // Get Category By Id --GET
        public async Task<IActionResult> Details(int id, int page = 1)
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName;
            ViewData["Language"] = language;
            TempData["CategoryId"] = id;
            var item = await _categoryRepository.GetCategoryWithProducts(language,id,null,page,3);
            if (item == null) return NotFound();
            return View(item);
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
                        //category.Image = Guid.NewGuid().ToString() + Path.GetExtension(category.PhotoFile.FileName);
                        category.Image = await FileSettings.UploadFile(category.PhotoFile, "Categories", _environment.WebRootPath);
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
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var catMapped = _mapper.Map<Category, CategoryVM>(category);
            return View(catMapped);
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
                       // categoryVM.Image = Guid.NewGuid().ToString() + Path.GetExtension(categoryVM.PhotoFile.FileName);
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
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var catMapped = _mapper.Map<Category, CategoryVM>(category);
            return View(catMapped);
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
                _unitOfWork.Repository<Category>().Delete(existCategory);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();

            foreach (var id in ids)
            {
                var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
                if (category != null)
                {
                    FileSettings.DeleteFile("Categories", category.Image, _environment.WebRootPath);
                    _unitOfWork.Repository<Category>().Delete(category);
                }
            }
            await _unitOfWork.Complet();
            return Ok();
        }

    }
}

