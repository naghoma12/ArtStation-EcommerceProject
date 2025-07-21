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
    public class SizeRepository : GenericRepository<ProductSize>, IProductTypeRepository<ProductSize>
    {
        private readonly ArtStationDbContext _context;

        public SizeRepository(ArtStationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<List<ProductSize>> GetProductTypes(int productId)
        {
            return await _context.ProductSizes
                .Where(x => x.IsActive && !x.IsDeleted && x.ProductId == productId)
                .ToListAsync();
        }
        public void DeleteRange(List<ProductSize> productSizes)
        {
            _context.ProductSizes.RemoveRange(productSizes);
        }
        public void UpdateRange(List<ProductSize> productSizes)
        {
            _context.ProductSizes.UpdateRange(productSizes);

        }
    }
}
