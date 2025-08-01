using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Repository.Repository
{
    public class PhotoRepository : GenericRepository<ProductPhotos>, IProductTypeRepository<ProductPhotos>
    {
        private readonly ArtStationDbContext _context;
        public PhotoRepository(ArtStationDbContext context): base(context)
        {
            _context = context;
        }
        public async void DeleteRange(List<ProductPhotos> productPhotos)
        {
           _context.ProductPhotos.RemoveRange(productPhotos);
        }

        public async Task<List<ProductPhotos>> GetProductTypes(int productId)
        {
            return await _context.ProductPhotos
               .Where(x => x.IsActive && !x.IsDeleted && x.ProductId == productId)
               .ToListAsync();
        }
        public void UpdateRange(List<ProductPhotos> productPhotos)
        {
            _context.ProductPhotos.UpdateRange(productPhotos);
        }
    }
}
