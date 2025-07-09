using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation_Dashboard.Helper;
using ArtStation_Dashboard.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core;
using Newtonsoft.Json;

namespace ArtStation_Dashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IForWhomRepository _forWhomRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IProductRepository productRepository,
            ICategoryRepository categoryRepository
            ,UserManager<AppUser> userManager,
            IForWhomRepository forWhomRepository
            ,IUnitOfWork unitOfWork
            ,IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
           _categoryRepository = categoryRepository;
            _userManager = userManager;
            _forWhomRepository = forWhomRepository;
            _unitOfWork = unitOfWork;
            _environment = webHostEnvironment;
        }
        // Admin
        public async Task<IActionResult> Index(int page = 1, int pageSize = 8)
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            ViewData["Language"] = language;

            var productsList = await _productRepository.GetProducts();

            var pagedProducts = productsList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedProducts = pagedProducts.Select(p =>
            {
                var price =  p.ProductSizes.Select(s => s.Price).DefaultIfEmpty(0).Min();
                var discountPercent = p.Sales
                    .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                    .Select(s => (decimal?)s.Discount)
                    .FirstOrDefault() ?? 0;

                var discountAmount = price * (discountPercent / 100m);
                return new SimpleProductVM
                {
                    Id = p.Id,
                    Name = (p.NameAR, p.NameEN).Localize(language),
                    Brand = (p.BrandAR, p.BrandEN).Localize(language),
                    CategoryName = (p.Category.NameAR, p.Category.NameEN).Localize(language),
                    Price = price,
                    PriceAfterSale = price - discountAmount,
                    Image = p.ProductPhotos.FirstOrDefault()?.Photo,
                    UserName = p.User?.FullName ?? "Unknown User"
                };
            }).ToList();

            var pageResult = new PagedResult<SimpleProductVM>
            {
                Items = mappedProducts,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = productsList.Count()
            };

            return View(pageResult);
        }
        // Trader
      //  [Authorize(Roles = "Trader")]
        public async Task<IActionResult> TraderProducts(int page = 1, int pageSize = 8)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            ViewData["Language"] = language;

            var productsList = await _productRepository.GetTraderProducts(userId);


            var pagedProducts = productsList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedProducts = pagedProducts.Select(p =>
            {
                var price = p.ProductSizes.Select(s => s.Price).DefaultIfEmpty(0).Min();
                var discountPercent = p.Sales
                    .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                    .Select(s => (decimal?)s.Discount)
                    .FirstOrDefault() ?? 0;

                var discountAmount = price * (discountPercent / 100m);
                return new SimpleProductVM
                {
                    Id = p.Id,
                    Name = (p.NameAR, p.NameEN).Localize(language),
                    Brand = (p.BrandAR, p.BrandEN).Localize(language),
                    CategoryName = (p.Category.NameAR, p.Category.NameEN).Localize(language),
                    Price = price,
                    PriceAfterSale = price - discountAmount,
                    Image = p.ProductPhotos.FirstOrDefault()?.Photo,
                    UserName = p.User?.FullName ?? "Unknown User"
                };
            }).ToList();

            var pageResult = new PagedResult<SimpleProductVM>
            {
                Items = mappedProducts,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = productsList.Count()
            };

            return View("Index",pageResult);
        }

        public async Task<IActionResult> Details(int id)
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            var product = await _productRepository.GetProductDetails(id,language);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            ViewData["Language"] = language;
            var productCreation = new ProductCreation()
            {
                Categories = await _categoryRepository.GetSimpleCategory(language),
                forWhoms = _forWhomRepository.GetForWhoms(language),
            };
            if (User.IsInRole("Admin"))
            {
                productCreation.Traders = await _userManager.GetUsersInRoleAsync("Trader");
            }
            return View(productCreation);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreation productCreation)
        {
            var language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(productCreation);
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var product = new Product
                {
                    NameAR = string.IsNullOrWhiteSpace(productCreation.NameAR) ? productCreation.NameEN : productCreation.NameAR,
                    NameEN = productCreation.NameEN,
                    DescriptionAR = productCreation.DescriptionAR,
                    DescriptionEN = productCreation.DescriptionEN,
                    BrandAR = productCreation.BrandAR,
                    BrandEN = productCreation.BrandEN,
                    ShippingDetailsAR = productCreation.ShippingDetailsAR,
                    ShippingDetailsEN = productCreation.ShippingDetailsEN,
                    DeliveredOnAR = productCreation.DeliveredOnAR,
                    DeliveredOnEN = productCreation.DeliveredOnEN,
                    UserId = User.IsInRole("Trader") ?
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) : (int)productCreation.TraderId,
                    CategoryId = productCreation.CategoryId,
                    SellersCount = 0
                };
                 _unitOfWork.Repository<Product>().Add(product);
                var count = await _unitOfWork.Complet();
                if (productCreation.Files.Any())
                {
                    foreach (var image in productCreation.Files)
                    {
                        var productPhoto = new ProductPhotos()
                        {                         
                            ProductId = product.Id
                        };
                        productPhoto.Photo = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        productPhoto.Photo = await FileSettings.UploadFile(image, "Products", _environment.WebRootPath);
                        _unitOfWork.Repository<ProductPhotos>().Add(productPhoto);
                    }
                }
                else
                {
                    ModelState.AddModelError("PhotoFiles", ("ادخل صور هذا المنتج","Product Photos is required").Localize(language));
                }
                if (productCreation.Colors.Any())
                { 
                        foreach (var color in productCreation.Colors)
                        {

                            ProductColor productColor = new ProductColor()
                            {
                                ProductId = product.Id,
                                HexCode = color.HexCode,
                                NameEN = color.ColorName,
                            };
                            _unitOfWork.Repository<ProductColor>().Add(productColor);
                        }
                    
                }
                if (productCreation.Flavours.Any())
                {
                    
                        foreach (var flavour in productCreation.Flavours)
                        {
                            ProductFlavour productFlavour = new ProductFlavour()
                            {
                                ProductId = product.Id,
                                NameAR = flavour.Name,
                                NameEN = flavour.Name
                            };
                            _unitOfWork.Repository<ProductFlavour>().Add(productFlavour);
                        }
                }
                //if (productCreation.SizeJson != null)
                //{
                //    var sizes = JsonConvert.DeserializeObject<List<ProductSize>>(productCreation.SizeJson);
                //    if (sizes?.Count > 0)
                //    {
                //        foreach (var size in sizes)
                //        {
                //            size.ProductId = product.Id;
                //            _unitOfWork.Repository<ProductSize>().Add(size);
                //        }
                //    }
                //}
                if (count > 0)
                {
                    ViewData["Message"] = "تم إضافة تفاصيل المنتج بنجاح";
                }
                return RedirectToAction("Index");
            }
            catch
            {

            }
            return View(productCreation);
        }

    }
}
