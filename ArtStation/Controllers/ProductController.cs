using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
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

        public ProductController(IProductRepository productRepository, IUnitOfWork unitOfWork)
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
                    return NotFound();
                }
                return Ok(products);

        }

        [HttpGet("GetNewProducts")]
        public async Task<IActionResult> GetNewProducts([FromHeader] string language, int? userId)
        {
            var products = await _productRepository.GetNewProducts(language, userId);
            if (products == null || !products.Any())
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpGet("GetProductOffers")]
        public async Task<IActionResult> GetProductOffers([FromHeader] string language)
        {
            var products = await _productRepository.GetProductOffers(language);
            if (products == null || !products.Any())
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpGet("GetBestSellerProducts")]
        public async Task<IActionResult> GetBestSellerProducts([FromHeader] string language, int? userId)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var products = await _productRepository.GetBestSellerProducts(language, userId);
            if (products == null || !products.Any())
            {
                return NotFound();
            }
            return Ok(products);

        }
       
        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> GetProductDetails(int id, [FromHeader] string language , int? userId)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var product = await _productRepository.GetProductById(language, id, userId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("SearchByProductName")]
        public async Task<IActionResult> SearchByProductName(string productName, [FromHeader] string language, int? userId)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                return BadRequest(new { message = "Product name cannot be empty." }); //Make it Arabic also
            }

            var products = await _productRepository.SearchByProductName(productName, language, userId);
            if (products == null || !products.Any())
            {
                return NotFound();
            }
            return Ok(products);
        }
    }
}
