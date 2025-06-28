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
        public async Task<IActionResult> GetNewProducts( string? token)
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
                return NotFound(new { Message = ControllerMessages.ProductNotFound});
            }
            return Ok(new
            {
                Message = ControllerMessages.ProductFound,
                Product = product
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
    }
}
