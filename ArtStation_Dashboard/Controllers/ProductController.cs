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
using ArtStation.Core.Helper;
using System.Drawing;
using AutoMapper;

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
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository,
            ICategoryRepository categoryRepository
            , UserManager<AppUser> userManager,
            IForWhomRepository forWhomRepository
            , IUnitOfWork unitOfWork
            , IWebHostEnvironment webHostEnvironment
            , IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _forWhomRepository = forWhomRepository;
            _unitOfWork = unitOfWork;
            _environment = webHostEnvironment;
            _mapper = mapper;
        }
        // Admin
        public async Task<IActionResult> Index(int page = 1, int pageSize = 7)
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

            return View(pageResult);
        }
        // Trader
        //  [Authorize(Roles = "Trader")]
        public async Task<IActionResult> TraderProducts(int page = 1, int pageSize = 7)
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

            return View("Index", pageResult);
        }

        public async Task<IActionResult> Details(int id)
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            var product = await _productRepository.GetProductDetails(id, language);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        private string GetLanguage()
        {
            return HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
        }
        private async Task<ProductCreation> FillForm(string language, ProductCreation productCreation)
        {

            productCreation.Categories = await _categoryRepository.GetSimpleCategory(language);
            productCreation.forWhoms = _forWhomRepository.GetForWhoms(language);
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                productCreation.Traders = await _userManager.GetUsersInRoleAsync("Trader");
            }
            else
            {
                productCreation.TraderName = user.FullName;
            }
            return productCreation;
        }
        private async Task AddFiles(List<IFormFile> formFiles, Product product, string language)
        {
            if (formFiles.Any())
            {
                foreach (var image in formFiles)
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
                ModelState.AddModelError("PhotoFiles", ("ادخل صور هذا المنتج", "Product Photos is required").Localize(language));
            }
        }
        private void AddColors(List<ColorVM> colors, Product product)
        {
            if (colors.Any())
            {
                foreach (var color in colors)
                {

                    ProductColor productColor = new ProductColor()
                    {
                        ProductId = product.Id,
                        HexCode = color.Hex,
                        NameEN = color.Name,
                        NameAR = color.Name
                    };
                    _unitOfWork.Repository<ProductColor>().Add(productColor);
                }
            }
        }

        private void AddSizes(List<SizeVM> sizes, Product product, string language)
        {
            if (sizes.Any())
            {
                foreach (var size in sizes)
                {
                    ProductSize productSize = new ProductSize()
                    {
                        SizeAR = size.SizeAR,
                        SizeEN = size.SizeEN,
                        Price = size.Price,
                        ProductId = product.Id
                    };
                    _unitOfWork.Repository<ProductSize>().Add(productSize);
                }
            }
            else
            {
                ModelState.AddModelError("Sizes", ("ادخل حجم وسعر هذا المنتج", "Product Size & Price is required").Localize(language));
            }
        }
        private void AddFlavours(List<FlavourVM> flavours, Product product)
        {
            if (flavours.Any())
            {
                foreach (var flavour in flavours)
                {
                    ProductFlavour productFlavour = new ProductFlavour()
                    {
                        ProductId = product.Id,
                        NameAR = flavour.FlavourAR,
                        NameEN = flavour.FlavourEN
                    };
                    _unitOfWork.Repository<ProductFlavour>().Add(productFlavour);
                }
            }
        }
        private void AddForWhoms(List<string> forWhoms, Product product)
        {
            if (forWhoms.Any())
            {
                foreach (var forwhom in forWhoms)
                {
                    ProductForWhom productForWhom = new ProductForWhom()
                    {
                        ProductId = product.Id,
                        ForWhomAR = Utility.GetForWhom(forwhom, "ar"),
                        ForWhomEN = Utility.GetForWhom(forwhom, "en"),
                    };
                    _unitOfWork.Repository<ProductForWhom>().Add(productForWhom);
                }
            }
        }
        private void AddSale(SaleVM saleVM, int productId)
        {
            if (saleVM != null)
            {
                Sale sale = new Sale
                {
                    ProductId = productId,
                    Discount = saleVM.Discount,
                    StartDate = saleVM.StartDate,
                    EndDate = saleVM.EndDate
                };
                _unitOfWork.Repository<Sale>().Add(sale);
            }
        }
        private async Task<List<SizeVM>> GetProductSizes(Product product, string language)
        {
            return product.ProductSizes.Select(s => new SizeVM
            {
                SizeAR = s.SizeAR,
                SizeEN = s.SizeEN,
                Price = s.Price
            }).ToList();
        }
        private async Task<List<ColorVM>> GetProductColors(Product product, string language)
        {
            return product.ProductColors.Select(s => new ColorVM
            {
                Name = s.NameEN,
                Hex = s.HexCode
            }).ToList();
        }
        private async Task<List<FlavourVM>> GetProductFlavours(Product product, string language)
        {
            return product.ProductFlavours.Select(s => new FlavourVM
            {
                FlavourAR = s.NameAR,
                FlavourEN = s.NameEN
            }).ToList();
        }
        private async Task<List<string>> GetProductPhotos(Product product, string language)
        {
            return product.ProductPhotos.Select(s => s.Photo
            ).ToList();
        }
        public async Task<IActionResult> Create()
        {
            string language = GetLanguage();
            ViewData["Language"] = language;
            var productCreation = new ProductCreation();
            productCreation = await FillForm(language, productCreation);
            return View(productCreation);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreation productCreation)
        {
            var language = GetLanguage();
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
                await AddFiles(productCreation.Files, product, language);
                AddColors(productCreation.Colors, product);
                AddForWhoms(productCreation.SelectedForWhoms, product);
                AddFlavours(productCreation.Flavours, product);
                AddSizes(productCreation.Sizes, product, language);
                AddSale(productCreation.Sale, product.Id);
                await _unitOfWork.Complet();
                if (count > 0)
                {
                    ViewData["Message"] = ("تم إضافة تفاصيل المنتج بنجاح", "Product details added successfully").Localize(language);
                }

                if(User.IsInRole("Trader")) return RedirectToAction("TraderProducts");
                else return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", ("حدث خطأ أثناء إضافة المنتج", "An error occurred while adding the product").Localize(language));
            }
            await FillForm(language, productCreation);
            return View(productCreation);
        }


        //[Authorize(AuthenticationSchemes = "Cookies", Roles = ("بائع,Admin"))]
        public async Task<IActionResult> Edit(int id)
        {
            var language = GetLanguage();
            ViewData["Message"] = language;
            var item = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (item == null)
            {
                ViewData["Message"] = ("لم يتم العثور على هذا العنصر", "This product not found").Localize(language);
                return RedirectToAction(nameof(Index));
            }
            var itemMapped = _mapper.Map<Product, ProductCreation>(item);
            itemMapped = await FillForm(language, itemMapped);
            itemMapped.Sizes = await GetProductSizes(item, language);
            itemMapped.Colors = await GetProductColors(item, language);
            itemMapped.Flavours = await GetProductFlavours(item, language);
            itemMapped.Images = await GetProductPhotos(item, language);
            return View(itemMapped);
        }

        //   [HttpPost]
        //   [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(ProductCreation productCreation)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return View(productCreation);
        //        }
        //         var ProMapped = _mapper.Map<ProductCreation, Product>(productCreation);
        //            _unitOfWork.Repository<Product>().Update(ProMapped);
        //            var count = await _unitOfWork.Complet();
        //            if (productCreation.Images.Any())
        //            {
        //                foreach (var file in productCreation.Images)
        //                {
        //                    ProductPhotos photos = new ProductPhotos();
        //                    photos.Photo = file.FileName;
        //                    photos.Photo = DocumentSetting.UploadFile(file, "products");
        //                    photos.ProductId = ProMapped.Id;
        //                    photos.FilePath = Path.Combine(_environment.ContentRootPath, "wwwroot\\Uploads\\products", photos.Photo);
        //                    if (User.IsInRole("Admin"))
        //                    {
        //                        photos.Enter = true;
        //                    }
        //                    else
        //                        photos.Enter = false;

        //                    _photo.Add(photos);
        //                    await _unitOfWork.Complet();
        //                }

        //            }


        //            if (productVM.ColorJson != null)
        //            {
        //                var colors = JsonConvert.DeserializeObject<List<string>>(productVM.ColorJson);
        //                if (colors.Count > 0)
        //                {
        //                    foreach (var hexCode in colors)
        //                    {

        //                        ProductColor productColor = new ProductColor()
        //                        {
        //                            ProductId = ProMapped.Id,
        //                            HexCode = hexCode,

        //                        };
        //                        _unitOfWork.Repository<ProductColor>().Add(productColor);
        //                        await _unitOfWork.Complet();

        //                    }
        //                }
        //            }


        //            if (productVM.SizeJson != null)
        //            {
        //                var sizes = JsonConvert.DeserializeObject<List<string>>(productVM.SizeJson);
        //                if (sizes?.Count > 0)
        //                {
        //                    foreach (var size in sizes)
        //                    {
        //                        ProductSize productSize = new ProductSize()
        //                        {
        //                            ProductId = ProMapped.Id,
        //                            Size = size
        //                        };
        //                        _unitOfWork.Repository<ProductSize>().Add(productSize);
        //                        await _unitOfWork.Complet();
        //                    }
        //                }
        //            }

        //            if (count > 0)
        //            {
        //                ViewData["Message"] = "تم تعديل تفاصيل المنتج بنجاح";
        //                return RedirectToAction(nameof(Details), new { id = ProMapped.Id });

        //            }



        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("CategoryId", "ادخل اسم القسم من فضلك");
        //            ModelState.AddModelError(string.Empty, ex.InnerException?.Message.ToString() ?? ex.Message.ToString());

        //        }
        //    }
        //    var List = await _unitOfWork.Repository<Category>().GetAllAsync();
        //    productVM.Categories = List;
        //    List<AppUser> users = new List<AppUser>();
        //    foreach (var item in _userManager.Users)
        //    {
        //        if (item.IsCompany == true)
        //        {
        //            users.Add(item);
        //        }
        //    }
        //    productVM.Users = users;
        //    return View(productVM);
        //}
        public async Task<IActionResult> Delete(int id)
        {
            var language = GetLanguage();
            var item = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (item == null)
            {
                ViewData["Message"] = ("لم يتم العثور على هذا العنصر", "This product not found").Localize(language);
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }
    }
}