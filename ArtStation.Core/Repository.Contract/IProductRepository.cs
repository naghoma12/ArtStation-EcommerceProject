using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IProductRepository:IGenericRepository<Product>
    {
         Task<IEnumerable<SimpleProduct>> GetAllProducts(string language , int? userId);
        Task<IEnumerable<SimpleProduct>> GetNewProducts(string language , int? userId);
        Task<IEnumerable<ProductOffers>> GetProductOffers(string language);
        Task<IEnumerable<SimpleProduct>> GetBestSellerProducts(string language, int? userId);
        Task<ProductDetailsDTO> GetProductById(string language, int id, int? userId);
        Task<IEnumerable<SimpleProduct>> SearchByProductName(string productName, string
            language, int? userId = null);

       Task<ProductWithPriceDto> GetProductWithPrice(int productId, int sizeId);
        Task<IEnumerable<SimpleProduct>> GetRelatedProducts(int productId, string language, int? userId = null);
        Task<IEnumerable<AIProducts>> GetAIProducts(string language);
        Task<IEnumerable<BrandDTO>> GetBrands(string language);
        Task<IEnumerable<SimpleProduct>> FilterProducts(List<SimpleProduct> products, int? minPriceRange, int? maxPriceRange, string? brand, bool men, bool women, bool kids, bool offer);
         Task<ProductsOFSpecificOrder> GetProductsOfSpecificOrder(int productId, int sizeId, int? flavourId, int? ColorId, string lang = "en");
    }
}
