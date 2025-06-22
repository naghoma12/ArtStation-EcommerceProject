using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllProducts([FromHeader] string language)
        {
                var products = await _productRepository.GetAllProducts(language);
                if (products == null || !products.Any())
                {
                    return NotFound();
                }
                return Ok(products);

        }

        [HttpGet("GetNewProducts")]
        public async Task<IActionResult> GetNewProducts([FromHeader] string language)
        {
            var products = await _productRepository.GetNewProducts(language);
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
        public async Task<IActionResult> GetBestSellerProducts([FromHeader] string language)
        {
            var products = await _productRepository.GetProductOffers(language);
            if (products == null || !products.Any())
            {
                return NotFound();
            }
            return Ok(products);
        }
    }
}
