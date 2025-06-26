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

        public async Task<CategoryWithProducts> GetCategoryById(string language, int id, int? userId = null)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id && !c.IsDeleted && c.IsActive)
                .Select(c => new CategoryWithProducts
                {
                    Id = c.Id,
                    Name = language == "en" ? c.NameEN : c.NameAR,
                    Products = c.Products
                        .Where(p => p.IsActive && !p.IsDeleted)
                        .Select(p => new SimpleProduct
                        {
                            Id = p.Id,
                            Name = language == "en" ? p.NameEN : p.NameAR,
                            PhotoUrl = p.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
                            ReviewsNumber = p.Reviews.Count(),
                            TotalPrice = p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0,
                            PriceAfterSale = (
                                (p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0) -
                                (((p.Sales
                                    .Where(s => s.IsActive && !s.IsDeleted &&
                                                s.StartDate <= DateTime.Now &&
                                                s.EndDate >= DateTime.Now)
                                    .OrderByDescending(s => s.Id)
                                    .Select(s => (int?)s.Discount)
                                    .FirstOrDefault() ?? 0) / 100m) *
                                    (p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0))
                            ),
                            IsActive = p.IsActive,
                            AvgRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : (float?)null,
                            IsFav = userId.HasValue && p.Favourites.Any(f => f.UserId == userId)
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return category;
        }
    }
}
