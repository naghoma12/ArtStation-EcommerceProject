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

        public async Task<CategoryWithProducts> GetCategoryById(string language, int id , int? userId = null)
        {
            var categories = await _context.Categories
                 .Where(c =>  c.Id == id
                 && !c.IsDeleted && c.IsActive)
                 .Select(c => new CategoryWithProducts
                 {
                     Id = c.Id,
                     Name = language == "en" ? c.NameEN : c.NameAR,
                     Products =
                 c.Products.Where(p => p.IsActive && !p.IsDeleted)
                 .Select(p => Utility.MapToSimpleProduct(p , userId , language))
                 .ToList()
                 })
                 .FirstOrDefaultAsync();
            return categories;
        }
    }
}
