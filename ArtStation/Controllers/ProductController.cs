using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtStation.Controllers
{

    public class ProductController : BaseController
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork unitOfWork;

        public ProductController(IProductRepository productRepository, IUnitOfWork unitOfWork
            )
        {
            _productRepository = productRepository;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(string? token)
        {
            int? userId = Utility.CheckToken(token);
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var products = await _productRepository.GetAllProducts(language, userId);
            if (products == null || !products.Any())
            {
                return Ok(new
                {
                    Message = ControllerMessages.ProductsListNotFound,
                    List = products
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.ProductsList,
                List = products
            });
        }

        [HttpGet("GetNewProducts")]
        public async Task<IActionResult> GetNewProducts(string? token)
        {
            int? userId = Utility.CheckToken(token);
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var products = await _productRepository.GetNewProducts(language, userId);
            if (products == null || !products.Any())
            {
                return Ok(new
                {
                    Message = ControllerMessages.ProductsListNotFound,
                    List = products
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.ProductsList,
                List = products
            });
        }

        [HttpGet("GetProductOffers")]
        public async Task<IActionResult> GetProductOffers()
        {
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var offers = await _productRepository.GetProductOffers(language);
            if (offers == null || !offers.Any())
            {
                return Ok(new
                {
                    Message = ControllerMessages.OffersNotFound,
                    List = offers
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.OffersList,
                List = offers
            });
        }

        [HttpGet("GetBestSellerProducts")]
        public async Task<IActionResult> GetBestSellerProducts(string? token)
        {
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            int? userId = Utility.CheckToken(token);
            var products = await _productRepository.GetBestSellerProducts(language, userId);

            if (products == null || !products.Any())
            {
                return Ok(new
                {
                    Message = ControllerMessages.ProductsListNotFound,
                    List = products
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.ProductsList,
                List = products
            });

        }

        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> GetProductDetails(int id, string? token)
        {
            int? userId = Utility.CheckToken(token);
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var product = await _productRepository.GetProductById(language, id, userId);
            if (product == null)
            {
                return NotFound(new { Message = ControllerMessages.ProductNotFound });
            }
            return Ok(new
            {
                Message = ControllerMessages.ProductFound,
                Product = product
            });
        }


        [HttpGet("GetMayAlsoKnowProducts")]
        public async Task<IActionResult> GetMayAlsoKnowProducts(int productId, string? token)
        {
            var product = await unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound(new { Message = ControllerMessages.ProductNotFound });
            }
            int? userId = Utility.CheckToken(token);
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var products = await _productRepository.GetRelatedProducts(productId, language, userId);
            if (products == null || !products.Any())
            {
                return Ok(new
                {
                    Message = ControllerMessages.ProductsListNotFound,
                    List = products
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.ProductsList,
                List = products
            });
        }

        [HttpGet("SearchByProductName")]
        public async Task<IActionResult> SearchByProductName(string? productName, string? token)
        {
            int? userId = Utility.CheckToken(token);
            var language = Request.Headers["Accept-Language"].ToString();

            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            if (string.IsNullOrWhiteSpace(productName))
            {
                return BadRequest(new
                {
                    Message = ControllerMessages.ProductNameSearch
                });
            }
            var products = await _productRepository.SearchByProductName(productName, language, userId);
            if (products == null || !products.Any())
            {
                return Ok(new
                {
                    Message = ControllerMessages.ProductNotFound,
                    List = products
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.ProductsList,
                List = products
            });
        }

        [HttpGet("GetAIProducts")]
        public async Task<IActionResult> GetAIProducts()
        {

            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var aiProducts = await _productRepository.GetAIProducts(language);
            if (aiProducts == null || !aiProducts.Any())
            {
                return Ok(new
                {
                    Message = ControllerMessages.ProductsListNotFound,
                    List = aiProducts
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.ProductsList,
                List = aiProducts
            });
        }
        [HttpGet("GetBrands")]
        public async Task<IActionResult> GetBrands()
        {
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var brands = await _productRepository.GetBrands(language);
            if (brands == null || !brands.Any())
            {
                return Ok(new
                {
                    Message = ControllerMessages.BrandsNotFound,
                    List = brands
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.BrandsList,
                List = brands
            });
        }

        [HttpGet("FilterProducts")]
        public async Task<IActionResult> FilterProducts(List<SimpleProduct> products, int? minPriceRange, int? maxPriceRange, string? brand, bool men = false, bool women = false, bool kids = false, bool offer = false)
        {
            var list = await _productRepository.FilterProducts(products, minPriceRange, maxPriceRange , brand , men , women , kids , offer);
            if (list == null || !list.Any())
            {
                return Ok(new
                {
                    Message = ControllerMessages.ProductsListNotFound,
                    List = list
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.ProductsList,
                List = list
            });
        }
    }

   
}
