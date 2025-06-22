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
         Task<IEnumerable<SimpleProduct>> GetAllProducts(string language);
        Task<IEnumerable<SimpleProduct>> GetNewProducts(string language);
        Task<IEnumerable<ProductOffers>> GetProductOffers(string language);
        Task<IEnumerable<SimpleProduct>> GetBestSellerProducts(string language);
        Task<ProductDetailsDTO> GetProductById(string language, int id);

    }
}
