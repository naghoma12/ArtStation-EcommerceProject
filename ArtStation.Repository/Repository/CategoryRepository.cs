using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ArtStationDbContext _context;

        public CategoryRepository(ArtStationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllCategories(string language)
        {
            return await _context.Categories
                .Where(c => c.Language == language && !c.IsDeleted && c.IsActive)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryById(string language, int id)
        {
            return await _context.Categories
                 .Where(c => c.Language == language
                 && c.Id == id
                 && !c.IsDeleted && c.IsActive)
                 .Include(c => c.Products.Select(p => new SimpleProduct
                 {
                     Id = p.Id,
                     Name = p.Name,
                     PhotoUrl = p.ProductPhotos.FirstOrDefault().Photo,
                     ReviewsNumber = p.Reviews.Count(),
                     TotalPrice = p.ProductSizes.Min(x => x.Size.Length),
                     // PriceAfterSale = 

                 }))
                 .FirstOrDefaultAsync();
        }
    }
}
