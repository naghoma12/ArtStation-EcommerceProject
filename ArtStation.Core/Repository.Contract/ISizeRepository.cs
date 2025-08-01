using ArtStation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IProductTypeRepository<T> where T : class
    {

        Task<List<T>> GetProductTypes(int productId);
         void DeleteRange(List<T> productSizes);
        void UpdateRange(List<T> productTypes);
    }
}
