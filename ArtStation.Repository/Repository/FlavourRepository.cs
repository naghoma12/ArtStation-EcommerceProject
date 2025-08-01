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
    public class FlavourRepository : GenericRepository<ProductFlavour>, IProductTypeRepository<ProductFlavour>
    {
        private readonly ArtStationDbContext _context;

        public FlavourRepository(ArtStationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<ProductFlavour>> GetProductTypes(int productId)
        {
            return await _context.ProductFlavours
                .Where(x => x.IsActive && !x.IsDeleted && x.ProductId == productId)
                .ToListAsync();
        }
        public void DeleteRange(List<ProductFlavour> productFlavours)
        {
            _context.ProductFlavours.RemoveRange(productFlavours);
        }
        public void UpdateRange(List<ProductFlavour> productFlavours)
        {
            _context.ProductFlavours.UpdateRange(productFlavours);
        }
    }
}
