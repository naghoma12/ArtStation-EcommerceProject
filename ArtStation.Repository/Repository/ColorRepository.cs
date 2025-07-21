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
    public class ColorRepository : GenericRepository<ProductColor> , IProductTypeRepository<ProductColor>
    {
        private readonly ArtStationDbContext _context;

        public ColorRepository(ArtStationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<ProductColor>> GetProductTypes(int productId)
        {
            return await _context.ProductColors
                .Where(x => x.IsActive && !x.IsDeleted && x.ProductId == productId)
                .ToListAsync();
        }
        public void DeleteRange(List<ProductColor> productColors)
        {
            _context.ProductColors.RemoveRange(productColors);
        }
        public void UpdateRange(List<ProductColor> productColors)
        {
            _context.ProductColors.UpdateRange(productColors);
        }
    }
}
