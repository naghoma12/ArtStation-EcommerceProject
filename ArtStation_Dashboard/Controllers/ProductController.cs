using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation_Dashboard.Helper;
using ArtStation_Dashboard.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace ArtStation_Dashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        //public async Task<IActionResult> Index()
        //{
        //    string language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "en";
        //    ViewData["Language"] = language;
        //    var List = await _productRepository.GetProducts();
        //    var products = List.Select(p => new SimpleProductVM
        //    {
        //        Id = p.Id,
        //        Name = (p.NameAR , p.NameEN).Localize(language),
        //        Brand = (p.BrandAR, p.BrandEN).Localize(language),
        //        CategoryName = (p.Category.NameAR, p.Category.NameEN).Localize(language),
        //        Price = p.ProductSizes.Min(p => p.Price),
        //        PriceAfterSale 
        //    });
        //    return View(List);
        //}
    }
}
