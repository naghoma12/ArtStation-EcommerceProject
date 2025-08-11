using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using ArtStation_Dashboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                    Image = $"http://artstationdashboard.runasp.net//Uploads//Categories/{c.Image}"
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
                Image = categoryData.Image,
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
        public async Task<PagedResult<Category>> GetFilteredAsync(
    Expression<Func<Category, bool>> filter = null,
    int page = 1,
    int pageSize = 5)
        {
            IQueryable<Category> query = _context.Set<Category>();

            if (filter != null)
                query = query.Where(filter);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Category>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task<CategoryProductsPagination> GetCategoryWithProducts(string language, int id, int? userId = null, int page = 1, int pageSize = 3)
        {
            var categoryData = await _context.Categories
                .Where(c => c.Id == id && !c.IsDeleted && c.IsActive)
                .Include(c => c.Products.Where(p => p.IsActive && !p.IsDeleted))
                    .ThenInclude(p => p.Sales)
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductSizes)
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductPhotos)
                .Include(c => c.Products)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync();
            if (categoryData == null) return null;

            var allProducts = categoryData.Products.ToList();
            var totalItems = allProducts.Count;

            var pagedProducts = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => 
                {
                    var price = p.ProductSizes.Select(s => s.Price).DefaultIfEmpty(0).Min();
                    var discountPercent = p.Sales
                        .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                        .Select(s => (decimal?)s.Discount)
                        .FirstOrDefault() ?? 0;

                    var discountAmount = price * (discountPercent / 100m);
                    return new SimpleProductVM
                    {
                        Id = p.Id,
                        Name = language == "ar" ? p.NameAR : p.NameEN,
                        Brand = language == "ar" ? p.BrandAR : p.BrandEN,
                        CategoryName = language == "ar" ? p.Category.NameAR : p.Category.NameEN,
                        Price = price,
                        PriceAfterSale = price - discountAmount,
                        Image = p.ProductPhotos.FirstOrDefault()?.Photo,
                        UserName = p.User?.FullName ?? "Unknown User"
                    };
                })
                .ToList();

            return new CategoryProductsPagination
            {
                Id = categoryData.Id,
                Name = language == "en" ? categoryData.NameEN : categoryData.NameAR,
                Image = categoryData.Image,
                ProductsPaged = new PagedResult<SimpleProductVM>
                {
                    Items = pagedProducts,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                }
            };
        }

    }
}
