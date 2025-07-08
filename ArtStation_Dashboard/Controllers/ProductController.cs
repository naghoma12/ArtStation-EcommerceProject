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

namespace ArtStation_Dashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
    }
}
