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
        public async Task<IEnumerable<CategoryDTO>> GetAllCategories(string language)
        {
            return await _context.Categories
                .Where(c =>!c.IsDeleted && c.IsActive)
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = language == "en" ? c.NameEN : c.NameAR,
                    Image = c.Image
                })
                .ToListAsync();
        }
        private int GetSale(Product product)
        {
            return product.Sales
                   .Where(s => s.IsActive && !s.IsDeleted &&
                   s.StartDate <= DateTime.Now &&
                   s.EndDate >= DateTime.Now)
                   .OrderByDescending(s => s.Id)
                   .Select(s => (int?)s.Discount)
                   .FirstOrDefault() ?? 0;
        }
        public async Task<CategoryWithProducts> GetCategoryById(string language, int id, int? userId = null)
        {
            var categoryData = await _context.Categories
                .Where(c => c.Id == id && !c.IsDeleted && c.IsActive)
                .Include(c => c.Products.Where(p => p.IsActive && !p.IsDeleted))
                    .ThenInclude(p => p.ProductSizes)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Sales)
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductPhotos)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Reviews)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Favourites)
                .Include(c => c.Products)
                    .ThenInclude(x => x.ForWhoms)
                .FirstOrDefaultAsync();

            if (categoryData == null) return null;

            var result = new CategoryWithProducts
            {
                Id = categoryData.Id,
                Name = language == "en" ? categoryData.NameEN : categoryData.NameAR,
                Products = categoryData.Products.Select(p => Utility.MapToSimpleProduct(p,userId , language)).ToList()
            };

            return result;
        }

        public async Task<IEnumerable<SimpleCategoryDTO>> GetSimpleCategory(string language)
        {
            return await _context.Categories
                .Where(c => c.IsActive && !c.IsDeleted)
                .Select(c => new SimpleCategoryDTO
                {
                    Id = c.Id,
                    Name = language == "en" ? c.NameEN : c.NameAR,
                })
                .ToListAsync();
                
        }
    }
}
