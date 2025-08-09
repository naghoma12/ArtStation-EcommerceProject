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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Build.Framework;

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
        private readonly IProductTypeRepository<ProductSize> _sizeRepository;
        private readonly IProductTypeRepository<ProductColor> _colorRepository;
        private readonly IProductTypeRepository<ProductFlavour> _flavourRepository;
        private readonly IProductTypeRepository<ProductPhotos> _photoRepository;
        private readonly IProductTypeRepository<ProductForWhom> _forwhomRepository;

        public ProductController(IProductRepository productRepository,
            ICategoryRepository categoryRepository
            , UserManager<AppUser> userManager,
            IForWhomRepository forWhomRepository
            , IUnitOfWork unitOfWork
            , IWebHostEnvironment webHostEnvironment
            , IMapper mapper,
           IProductTypeRepository<ProductSize> sizeRepository
            , IProductTypeRepository<ProductColor> colorRepository
            , IProductTypeRepository<ProductFlavour> flavourRepository
            , IProductTypeRepository<ProductPhotos> photoRepository
            , IProductTypeRepository<ProductForWhom> forwhomRepository
            )
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _forWhomRepository = forWhomRepository;
            _unitOfWork = unitOfWork;
            _environment = webHostEnvironment;
            _mapper = mapper;
            _sizeRepository = sizeRepository;
            _colorRepository = colorRepository;
           _flavourRepository = flavourRepository;
            _photoRepository = photoRepository;
            _forwhomRepository = forwhomRepository;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ProductSections()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InActiveProducts(int page = 1, int pageSize = 5)
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            ViewData["Language"] = language;

            var productsList = await _productRepository.GetInActiveProducts();


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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateProduct(int id)
        {
            var product = await _productRepository.GetInActiveProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            product.IsActive = true;
            _unitOfWork.Repository<Product>().Update(product);
            var count = await _unitOfWork.Complet();
            if (count > 0)
            {
                TempData["Message"] = ("تم تفعيل المنتج بنجاح", "Product activated successfully").Localize(GetLanguage());
            }
            else
            {
                TempData["Message"] = ("حدث خطأ أثناء تفعيل المنتج", "An error occurred while activating the product").Localize(GetLanguage());
            }
            return RedirectToAction(nameof(InActiveProducts), new { page = 1, pageSize = 5 });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletedProducts(int page = 1, int pageSize = 5)
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            ViewData["Language"] = language;

            var productsList = await _productRepository.GetDeletedProducts();


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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            var product = await _productRepository.GetDeletedProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            product.IsDeleted = false;
            _unitOfWork.Repository<Product>().Update(product);
            var count = await _unitOfWork.Complet();
            if (count > 0)
            {
                TempData["Message"] = ("تم إسترجاع المنتج بنجاح", "Product Restored successfully").Localize(GetLanguage());
            }
            else
            {
                TempData["Message"] = ("حدث خطأ أثناء تفعيل المنتج", "An error occurred while activating the product").Localize(GetLanguage());
            }
            return RedirectToAction(nameof(DeletedProducts), new { page = 1, pageSize = 5 });
        }
        // Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FilterProducts(int? categoryId, string searchText, int page = 1, int pageSize = 5 )
        {
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            ViewData["Language"] = language;

            var productsList = await _productRepository.GetProducts();
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsList = productsList.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            if (!string.IsNullOrEmpty(searchText))
                productsList = productsList.Where(p => p.NameAR.Contains(searchText) || p.NameEN.ToLower().Contains(searchText.ToLower()));

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

            return PartialView("PartialView/_Product" , pageResult);
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Index()
        {
            ViewData["Language"] = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            var categories = await _categoryRepository.GetSimpleCategory(GetLanguage());
            return View(categories);
        }
        [Authorize(Roles = "Trader")]
        public async Task<IActionResult> GetTraderCategory()
        {
            ViewData["Language"] = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            var categories = await _categoryRepository.GetSimpleCategory(GetLanguage());
            return View(categories);
        }
        // Trader
        [Authorize(Roles = "Trader")]
        public async Task<IActionResult> FilterTraderProducts(int? categoryId, string searchText,int page = 1, int pageSize =5)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
            ViewData["Language"] = language;

            var productsList = await _productRepository.GetTraderProducts(userId);

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsList = productsList.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            if (!string.IsNullOrEmpty(searchText))
                productsList = productsList.Where(p => p.NameAR.Contains(searchText) || p.NameEN.ToLower().Contains(searchText.ToLower()));

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

            return PartialView("PartialView/_Product", pageResult);
        }
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> DisableProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.IsActive = false;
            _unitOfWork.Repository<Product>().Update(product);
            var count = await _unitOfWork.Complet();
            if (count > 0)
            {
                TempData["Message"] = ("تم تعطيل المنتج بنجاح", "Product disabled successfully").Localize(GetLanguage());
            }
            else
            {
                TempData["Message"] = ("حدث خطأ أثناء تعطيل المنتج", "An error occurred while disabling the product").Localize(GetLanguage());
            }
            return RedirectToAction(nameof(Index), new { page = 1, pageSize = 5 });
        }

        #region Methods

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
        private async Task AddFiles(List<IFormFile> formFiles, Product product)
        {
            if (formFiles.Any())
            {
                foreach (var image in formFiles)
                {
                    var productPhoto = new ProductPhotos()
                    {
                        ProductId = product.Id
                    };
                    //productPhoto.Photo = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    productPhoto.Photo = await FileSettings.UploadFile(image, "Products", _environment.WebRootPath);
                    _unitOfWork.Repository<ProductPhotos>().Add(productPhoto);
                }
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
                        NameEN = color.NameEN,
                        NameAR = color.NameAR
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
                        Price = (decimal)size.Price,
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
            if (saleVM.Discount != null)
            {
                Sale sale = new Sale
                {
                    ProductId = productId,
                    Discount = (int)saleVM.Discount,
                    StartDate = (DateTime)saleVM.StartDate,
                    EndDate = (DateTime)saleVM.EndDate
                };
                _unitOfWork.Repository<Sale>().Add(sale);
            }
        }
        private async Task<List<SizeVM>> GetProductSizes(Product product, string language)
         {
            return product.ProductSizes.Select(s => new SizeVM
            {
                Id = s.Id,
                SizeAR = s.SizeAR,
                SizeEN = s.SizeEN,
                Price = s.Price
            }).ToList();
        }
        private async Task<List<ColorVM>> GetProductColors(Product product, string language)
        {
            return product.ProductColors.Select(s => new ColorVM
            {
                Id = s.Id,
                NameEN = s.NameEN,
                NameAR = s.NameAR,
                Hex = s.HexCode
            }).ToList();
        }
        private async Task<List<FlavourVM>> GetProductFlavours(Product product, string language)
        {
            return product.ProductFlavours.Select(s => new FlavourVM
            {
                Id = s.Id,
                FlavourAR = s.NameAR,
                FlavourEN = s.NameEN
            }).ToList();
        }
        private async Task<List<string>> GetProductPhotos(Product product, string language)
        {
            return product.ProductPhotos.Select(s => s.Photo
            ).ToList();
        }
        private async Task HandleSizes(List<SizeVM> sizes, Product product)
        {
            if (sizes.Any())
            {
                var existingSizes = await _sizeRepository.GetProductTypes(product.Id);
                foreach (var submittedSize in sizes)
                {
                    if (submittedSize.Id != 0 && submittedSize.SizeEN != null && submittedSize.SizeAR != null)
                    {
                        // Update existing
                        var existing = existingSizes.FirstOrDefault(s => s.Id == submittedSize.Id);
                        if (existing != null)
                        {
                            if (existing.SizeAR != submittedSize.SizeAR || existing.SizeEN != submittedSize.SizeEN || existing.Price != (decimal)submittedSize.Price)
                            {
                                existing.SizeAR = submittedSize.SizeAR;
                                existing.SizeEN = submittedSize.SizeEN;
                                existing.Price = (decimal)submittedSize.Price;
                                _unitOfWork.Repository<ProductSize>().Update(existing);
                            }
                        }

                    }
                    else if (submittedSize.Id == 0 && submittedSize.SizeEN != null && submittedSize.SizeAR != null)
                    {
                        // New size -> add to DB
                        var newSize = new ProductSize
                        {
                            SizeAR = submittedSize.SizeAR,
                            SizeEN = submittedSize.SizeEN,
                            Price = (decimal)submittedSize.Price,
                            ProductId = product.Id
                        };
                        _unitOfWork.Repository<ProductSize>().Add(newSize);
                    }
                }

                var submittedIds = sizes
                    .Where(s => s.Id != 0 && s.SizeEN == null && s.SizeAR == null)
                    .Select(s => s.Id)
                    .ToList();

                var toDelete = existingSizes
                .Where(e => submittedIds.Contains(e.Id))
                    .ToList();
                if (toDelete.Count > 0)
                {
                    _sizeRepository.DeleteRange(toDelete);

                }
            }
        }
        private async Task HandleColors(List<ColorVM> colors, Product product)
        {
            if (colors.Any())
            {
                var existingColors = await _colorRepository.GetProductTypes(product.Id);
                foreach (var submittedColor in colors)
                {
                    var existing = existingColors.FirstOrDefault(s => s.NameEN == submittedColor.NameEN
                        && s.NameAR == submittedColor.NameAR && s.HexCode == submittedColor.Hex);
                    if (existing == null)
                    {
                        var newColor = new ProductColor
                        {
                            NameAR = submittedColor.NameAR,
                            NameEN = submittedColor.NameEN,
                            HexCode = submittedColor.Hex,
                            ProductId = product.Id
                        };
                        _unitOfWork.Repository<ProductColor>().Add(newColor);
                    }

                }
                var toDelete = existingColors
                    .Where(s => !colors.Any(
                        sub => sub.NameAR == s.NameAR
                        && sub.NameEN == s.NameEN
                        && sub.Hex == s.HexCode))
                    .ToList();

                if (toDelete.Count > 0)
                {
                    _colorRepository.DeleteRange(toDelete);

                }
            }
        }
        private async Task HandleFlavours(List<FlavourVM> flavours, Product product)
        {
            if (flavours.Any())
            {
                var existingFlavours = await _flavourRepository.GetProductTypes(product.Id);
                foreach (var submittedFlavour in flavours)
                {
                    if (submittedFlavour.Id != 0 && submittedFlavour.FlavourAR != null && submittedFlavour.FlavourEN != null)
                    {
                        // Update existing
                        var existing = existingFlavours.FirstOrDefault(s => s.Id == submittedFlavour.Id);
                        if (existing != null)
                        {
                            if (existing.NameAR != submittedFlavour.FlavourAR || existing.NameEN != submittedFlavour.FlavourEN)
                            {
                                existing.NameAR = submittedFlavour.FlavourAR;
                                existing.NameEN = submittedFlavour.FlavourEN;
                                _unitOfWork.Repository<ProductFlavour>().Update(existing);

                            }
                        }
                    }
                    else if (submittedFlavour.Id == 0 && submittedFlavour.FlavourAR != null && submittedFlavour.FlavourEN != null)
                    {
                        // New flavpur -> add to DB
                        var newFlavour = new ProductFlavour
                        {
                            NameAR = submittedFlavour.FlavourAR,
                            NameEN = submittedFlavour.FlavourEN,
                            ProductId = product.Id
                        };
                        _unitOfWork.Repository<ProductFlavour>().Add(newFlavour);
                    }
                }

                var deletedIds = flavours
                    .Where(s => s.Id != 0 && s.FlavourEN == null && s.FlavourAR == null)
                    .Select(s => s.Id)
                    .ToList();

                var toDelete = existingFlavours
                .Where(e => deletedIds.Contains(e.Id))
                    .ToList();
                if (toDelete.Count > 0)
                {
                    _flavourRepository.DeleteRange(toDelete);

                }
            }
        }
        private async Task HandleSale(SaleVM sale, Product product)
        {
            if (sale.Id == null && sale.Discount > 0)
            {
                _unitOfWork.Repository<Sale>().Add(new Sale
                {
                    ProductId = product.Id,
                    Discount = (int)sale.Discount,
                    StartDate = (DateTime)sale.StartDate,
                    EndDate = (DateTime)sale.EndDate
                });
            }
            if (sale.Discount == 0 && sale.Id > 0)
            {
                var exsistsale = await _unitOfWork.Repository<Sale>().GetByIdAsync((int)sale.Id);
                if (exsistsale != null)
                {
                    exsistsale.IsDeleted = true;
                    _unitOfWork.Repository<Sale>().Update(exsistsale);
                }
            }
            if (sale.Id > 0 && sale.Discount > 0)
            {
                var exsistsale = await _unitOfWork.Repository<Sale>().GetByIdAsync((int)sale.Id);
                if (exsistsale != null)
                {
                    exsistsale.Discount = (int)sale.Discount;
                    exsistsale.StartDate = (DateTime)sale.StartDate;
                    exsistsale.EndDate = (DateTime)sale.EndDate;
                    _unitOfWork.Repository<Sale>().Update(exsistsale);
                }
            }
        }
        private async Task HandleImages(List<IFormFile> files, Product product, List<string> deletedPhotos)
        {
            await AddFiles(files, product);

            if (deletedPhotos.Any())
            {
                var existingPhotos = await _photoRepository.GetProductTypes(product.Id);
                foreach (var photo in existingPhotos)
                {
                    if (deletedPhotos.Contains(photo.Photo))
                    {
                        FileSettings.DeleteFile("Products", photo.Photo, _environment.WebRootPath);
                        _unitOfWork.Repository<ProductPhotos>().Delete(photo);
                    }
                }
            }
        }
        private async Task HandleForWhom(List<string> SelectedForWhoms, Product product)
        {
            var existingForWhoms = await _forwhomRepository.GetProductTypes(product.Id);

            var submittedSet = new HashSet<string>(SelectedForWhoms);
            foreach (var existing in existingForWhoms)
            {
                var existsInSubmitted = submittedSet.Contains(existing.ForWhomEN) || submittedSet.Contains(existing.ForWhomAR);

                if (!existsInSubmitted)
                {
                    _unitOfWork.Repository<ProductForWhom>().Delete(existing);
                }
            }
            foreach (var submittedForWhom in submittedSet)
            {
                var existsInDb = existingForWhoms.Any(f => f.ForWhomEN == submittedForWhom || f.ForWhomAR == submittedForWhom);

                if (!existsInDb)
                {
                    var newForWhom = new ProductForWhom
                    {
                        ProductId = product.Id,
                        ForWhomAR = Utility.GetForWhom(submittedForWhom, "ar"),
                        ForWhomEN = Utility.GetForWhom(submittedForWhom, "en")
                    };

                    _unitOfWork.Repository<ProductForWhom>().Add(newForWhom);
                }
            }
        }
        #endregion
        [Authorize]
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
        [Authorize(Roles = ("Trader,Admin"))]
        public async Task<IActionResult> Create(ProductCreation productCreation)
        {
            var language = GetLanguage();
            try
            {
                if (!ModelState.IsValid)
                {
                    productCreation = await FillForm(language, productCreation);
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
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) : (int)productCreation.UserId,
                    CategoryId = productCreation.CategoryId,
                    SellersCount = 0
                };
                _unitOfWork.Repository<Product>().Add(product);
                var count = await _unitOfWork.Complet();
                await AddFiles(productCreation.Files, product);
                AddColors(productCreation.Colors, product);
                AddForWhoms(productCreation.SelectedForWhoms, product);
                AddFlavours(productCreation.Flavours, product);
                AddSizes(productCreation.Sizes, product, language);
                AddSale(productCreation.Sale, product.Id);
                await _unitOfWork.Complet();
                if (count > 0)
                {
                    TempData["Message"] = ("تم إضافة تفاصيل المنتج بنجاح", "Product details added successfully").Localize(language);
                }

                return RedirectToAction(nameof(Details), new { id = product.Id });
            }
            catch
            {
                ModelState.AddModelError("", ("حدث خطأ أثناء إضافة المنتج", "An error occurred while adding the product").Localize(language));
            }
            productCreation = await FillForm(language, productCreation);
            return View(productCreation);
        }


        [Authorize(Roles = ("Trader,Admin"))]
        public async Task<IActionResult> Edit(int id)
        {
            var language = GetLanguage();
            ViewData["Message"] = language;
            var item = await _productRepository.GetProductAsync(id);
            if (item == null)
            {
                TempData["Message"] = ("لم يتم العثور على هذا العنصر", "This product not found").Localize(language);
                return RedirectToAction(nameof(Index));
            }

            ProductCreation product = new ProductCreation()
            {
                Id = item.Id,
                NameAR = item.NameAR,
                NameEN = item.NameEN,
                DescriptionAR = item.DescriptionAR,
                SellersCount = item.SellersCount,
                DescriptionEN = item.DescriptionEN,
                BrandAR = item.BrandAR,
                BrandEN = item.BrandEN,
                ShippingDetailsAR = item.ShippingDetailsAR,
                ShippingDetailsEN = item.ShippingDetailsEN,
                DeliveredOnAR = item.DeliveredOnAR,
                DeliveredOnEN = item.DeliveredOnEN,
                CategoryId = item.CategoryId,
                UserId = item.UserId,
                Sale = item.Sales.FirstOrDefault(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now) != null
                    ? new SaleVM
                    {
                        Id = item.Sales.FirstOrDefault(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now).Id,
                        Discount = item.Sales.FirstOrDefault(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now).Discount,
                        StartDate = item.Sales.FirstOrDefault(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now).StartDate,
                        EndDate = item.Sales.FirstOrDefault(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now).EndDate
                    }
                    : null,
                Colors = item.ProductColors.Select(c => new ColorVM
                {
                    Id = c.Id,
                    NameEN = c.NameEN,
                    NameAR = c.NameAR,
                    Hex = c.HexCode
                }).ToList(),
                Sizes = item.ProductSizes.Select(s => new SizeVM
                {
                    Id = s.Id,
                    SizeAR = s.SizeAR,
                    SizeEN = s.SizeEN,
                    Price = s.Price
                }).ToList(),
                Flavours = item.ProductFlavours.Select(f => new FlavourVM
                {
                    Id = f.Id,
                    FlavourAR = f.NameAR,
                    FlavourEN = f.NameEN
                }).ToList(),
                SelectedForWhoms = item.ForWhoms.Select(f => language == "en" ? f.ForWhomEN : f.ForWhomAR).ToList(),
                Images = item.ProductPhotos.Select(p => p.Photo).ToList(),
            };
            product = await FillForm(language, product);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        [DisableRequestSizeLimit]
        [Authorize(Roles = ("Trader,Admin"))]
        public async Task<IActionResult> Edit(ProductCreation productCreation)
        {
                var language = GetLanguage();
            var Images = await _photoRepository.GetProductTypes(productCreation.Id);
            try
            {
                ModelState.Remove("Sale");
                if (!ModelState.IsValid)
                {
                    if (Images.Any())
                    {
                        productCreation.Images = Images.Select(p => p.Photo).ToList();
                    }
                    else
                    {
                        productCreation.Images = new List<string>();
                    }
                    productCreation = await FillForm(language, productCreation);
                    return View(productCreation);
                }
                var ProMapped = _mapper.Map<ProductCreation, Product>(productCreation);
                _unitOfWork.Repository<Product>().Update(ProMapped);

                //For Size 
                await HandleSizes(productCreation.Sizes, ProMapped);

                //For Colors
                await HandleColors(productCreation.Colors, ProMapped);

                //For Flavours
                await HandleFlavours(productCreation.Flavours, ProMapped);

                //For Sale
               await HandleSale(productCreation.Sale, ProMapped);

                //For Images
                await HandleImages(productCreation.Files, ProMapped, productCreation.DeletedPhotos);
                //For ForWhoms
                await HandleForWhom(productCreation.SelectedForWhoms, ProMapped);

               var count = await _unitOfWork.Complet();
                if (count > 0)
                {
                    TempData["Message"] = ("تم تعديل تفاصيل المنتج بنجاح", "Product details updated successfully").Localize(GetLanguage());
                    return RedirectToAction(nameof(Details), new { id = ProMapped.Id });

                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserId", ("ادخل اسم الشركه من فضلك","Please select company").Localize(language));

            }
            if (Images.Any())
            {
                productCreation.Images = Images.Select(p => p.Photo).ToList();
            }
            else
            {
                productCreation.Images = new List<string>();
            }
            productCreation = await FillForm(language, productCreation);
            return View(productCreation);
        }
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return await Details(id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(ProductDetailsVM productDetails)
        {
            var language = GetLanguage();
            try
            {
                var product = await _productRepository.GetProductAsync(productDetails.Id);
                product.IsDeleted = true;
                _unitOfWork.Repository<Product>().Update(product);
                if (product.ProductSizes.Any())
                {
                    foreach (var size in product.ProductSizes)
                    {
                        size.IsDeleted = true;
                    }
                    _sizeRepository.UpdateRange(product.ProductSizes);
                }
                if (product.ProductColors.Any())
                {
                    foreach (var color in product.ProductColors)
                    {
                        color.IsDeleted = true;
                    }
                    _colorRepository.UpdateRange(product.ProductColors);
                }
                if (product.ProductFlavours.Any())
                {
                    foreach (var flavour in product.ProductFlavours)
                    {
                        flavour.IsDeleted = true;
                    }
                    _flavourRepository.UpdateRange(product.ProductFlavours);
                }
                if (product.ForWhoms.Any())
                {
                    foreach (var forwhom in product.ForWhoms)
                    {
                        forwhom.IsDeleted = true;
                    }
                    _forwhomRepository.UpdateRange(product.ForWhoms);
                }
                if (product.ProductPhotos.Any())
                {
                    foreach (var photo in product.ProductPhotos)
                    {
                        FileSettings.DeleteFile("Products", photo.Photo, _environment.WebRootPath);
                        photo.IsDeleted = true;
                    }
                    _photoRepository.UpdateRange(product.ProductPhotos);
                }
                if (product.Sales.Any())
                {
                    foreach (var sales in product.Sales)
                    {
                        sales.IsDeleted = true;
                    }
                }
                var count = await _unitOfWork.Complet();
                if (count > 0)
                {
                    TempData["Message"] = ("تم حذف المنتج بنجاح", "Product deleted successfully").Localize(language);
                    return RedirectToAction(nameof(Index), new { page = 1, pageSize = 5});
                }
            }
            catch
            {
                ViewData["Message"] = ("حدث خطأ أثناء حذف المنتج", "An error occurred while deleting the product").Localize(language);
            }
            return View(productDetails);
        }

        }
    }