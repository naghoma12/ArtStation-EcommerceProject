using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using ArtStation.Resources;
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
        public async Task<IActionResult> GetAllProducts([FromHeader] string language, int? userId)
        {
                var products = await _productRepository.GetAllProducts(language, userId);
                if (products == null || !products.Any())
                {
                return Ok(new
                {
                    Message = "No products found.",
                    List = products
                });
                }
                return Ok(new
                {
                    Message = "List of Products.",
                    List = products
                });
        }

        [HttpGet("GetNewProducts")]
        public async Task<IActionResult> GetNewProducts([FromHeader] string language, int? userId)
        {
            var products = await _productRepository.GetNewProducts(language, userId);
            if (products == null || !products.Any())
            {
                return Ok(new
                {
                    Message = "No products found.",
                    List = products
                });
            }
            return Ok(new
            {
                Message = "List of Products.",
                List = products
            });
        }

        [HttpGet("GetProductOffers")]
        public async Task<IActionResult> GetProductOffers([FromHeader] string language)
        {
            var offers = await _productRepository.GetProductOffers(language);
            if (offers == null || !offers.Any())
            {
                return Ok(new
                {
                    Message = "No Offers found.",
                    List = offers
                });
            }
            return Ok(new
            {
                Message = "List of Offers.",
                List = offers
            });
        }

        [HttpGet("GetBestSellerProducts")]
        public async Task<IActionResult> GetBestSellerProducts([FromHeader] string language, int? userId)
        {
            var products = await _productRepository.GetBestSellerProducts(language, userId);

            if (products == null || !products.Any())
            {
                return Ok(new
                {
                    Message = "No products found.",
                    List = products
                });
            }
            return Ok(new
            {
                Message = "List of Products.",
                List = products
            });

        }
       
        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> GetProductDetails(int id, [FromHeader] string language , int? userId)
        {
            var product = await _productRepository.GetProductById(language, id, userId);
            if (product == null)
            {
                return NotFound(new { Message = $"There is no product with this ID : {id}" });
            }
            return Ok(new
            {
                Message = "Product found successfully.",
                Product = product
            });
        }

        [HttpGet("SearchByProductName")]
        public async Task<IActionResult> SearchByProductName(string? productName, [FromHeader] string language, int? userId)
        {
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
                    Message = "No products with this name.",
                    List = products
                });
            }
            return Ok(new
            {
                Message = "List of Products.",
                List = products
            });
        }
    }
}
