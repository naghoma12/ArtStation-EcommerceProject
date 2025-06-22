using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IProductRepository
    {
         Task<IEnumerable<SimpleProduct>> GetAllProducts(string language , int? userId);
        Task<IEnumerable<SimpleProduct>> GetNewProducts(string language , int? userId);
        Task<IEnumerable<ProductOffers>> GetProductOffers(string language);
        Task<IEnumerable<SimpleProduct>> GetBestSellerProducts(string language, int? userId);
        Task<ProductDetailsDTO> GetProductById(string language, int id, int? userId);

    }
}
