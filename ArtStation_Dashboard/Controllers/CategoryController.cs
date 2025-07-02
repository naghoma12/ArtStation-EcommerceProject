using ArtStation.Core.Entities;
using ArtStation.Core;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ArtStation_Dashboard.ViewModels;
using ArtStation_Dashboard.Helper;
using Microsoft.AspNetCore.Localization;

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
        public async Task<IActionResult> Index()
        {
            try
            {
                string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName;
                ViewData["Language"] = language;
                var List = await _unitOfWork.Repository<Category>().GetAllAsync();
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
            var item = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (item == null) return RedirectToAction(nameof(Index));
            var itemMapped = _mapper.Map<Category, CategoryVM>(item);

            return View(itemMapped);
        }

        //Open the form --Get
        [HttpGet]
        public IActionResult Create()
        {
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
                        category.PhotoURL = Guid.NewGuid().ToString() + Path.GetExtension(category.PhotoFile.FileName);
                        await FileSettings.UploadFile(category.PhotoFile, "Categories", _environment.WebRootPath);
                    }
                    //else
                    //{
                    //    ModelState.AddModelError("Image", "Please Enter Photo");
                    //}
                    var CatMapped = _mapper.Map<CreatedCategory, Category>(category);
                    _unitOfWork.Repository<Category>().Add(CatMapped);
                    var count = await _unitOfWork.Complet();

                    if (count > 0)
                        TempData["message"] = "تم إضافة تفاصيل القسم بنجاح"; 
                    else
                        TempData["message"] = "فشلت عملية الإضافه";
                    return RedirectToAction(nameof(Index));
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
                        FileSettings.DeleteFile("Categories",categoryVM.PhotoURL,_environment.WebRootPath);
                        categoryVM.PhotoURL = Guid.NewGuid().ToString() + Path.GetExtension(categoryVM.PhotoFile.FileName);
                        await FileSettings.UploadFile(categoryVM.PhotoFile, "Categories", _environment.WebRootPath);
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
            return await Details(id);
        }

        //Delete category --post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CategoryVM categoryVM)
        {
            try
            {
                var category = _mapper.Map<CategoryVM, Category>(categoryVM);
                FileSettings.DeleteFile("Categories", categoryVM.PhotoURL, _environment.WebRootPath);
                category.IsDeleted = true;
                category.IsActive = false;
                _unitOfWork.Repository<Category>().Update(category);
                var count = await _unitOfWork.Complet();
                if (count > 0)
                {
                    TempData["Message"] = "تم حذف تفاصيل القسم بنجاح";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                TempData["Message"] = "فشلت عملية الحذف";
            }
               return View(categoryVM);
        }

    }
}

