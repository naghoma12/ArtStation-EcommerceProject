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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ArtStationDbContext _context;

        public ProductRepository(ArtStationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SimpleProduct>> GetAllProducts(string language)
        {
            var products = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted && p.Language == language)
                .Select(p => new SimpleProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    PhotoUrl = p.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
                    ReviewsNumber = p.Reviews.Count(),
                    TotalPrice = p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0,
                    PriceAfterSale = p.ProductSizes.Any()
                ? (
                    (p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
                    - (
                        (p.Sales
                            .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                            .Any()
                            ? (
                                (p.Sales
                                    .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                                    .OrderByDescending(s => s.Id)
                                    .Select(s => (int?)s.Discount)
                                    .FirstOrDefault() ?? 0)
                            )
                            : 0
                        ) / 100m
                        * (p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
                    )
                )
                : 0,
                    IsActive = p.IsActive,
                    AvgRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : (float?)null,
                    IsFav = false
                })
                .ToListAsync();
            return products;
        }

        public async Task<IEnumerable<SimpleProduct>> GetBestSellerProducts(string language)
        {
                var products = await _context.Products
                    .Where(p => p.IsActive && !p.IsDeleted && p.Language == language)
                    .OrderByDescending(x => x.SellersCount)
                    .Select(p => new SimpleProduct
                    {
                        Id = p.Id,
                        Name = p.Name,
                        PhotoUrl = p.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
                        ReviewsNumber = p.Reviews.Count(),
                        TotalPrice = p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0,
                        PriceAfterSale = p.ProductSizes.Any()
                    ? (
                        (p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
                        - (
                            (p.Sales
                                .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                                .Any()
                                ? (
                                    (p.Sales
                                        .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                                        .OrderByDescending(s => s.Id)
                                        .Select(s => (int?)s.Discount)
                                        .FirstOrDefault() ?? 0)
                                )
                                : 0
                            ) / 100m
                            * (p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
                        )
                    )
                    : 0,
                        IsActive = p.IsActive,
                        AvgRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : (float?)null,
                        IsFav = false
                    })
                    .ToListAsync();
                return products;
            
        }

        public async Task<IEnumerable<SimpleProduct>> GetNewProducts(string language)
        {
            var products = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted && p.Language == language)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new SimpleProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    PhotoUrl = p.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
                    ReviewsNumber = p.Reviews.Count(),
                    TotalPrice = p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0,
                    PriceAfterSale = p.ProductSizes.Any()
                ? (
                    (p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
                    - (
                        (p.Sales
                            .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                            .Any()
                            ? (
                                (p.Sales
                                    .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                                    .OrderByDescending(s => s.Id)
                                    .Select(s => (int?)s.Discount)
                                    .FirstOrDefault() ?? 0)
                            )
                            : 0
                        ) / 100m
                        * (p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
                    )
                )
                : 0,
                    IsActive = p.IsActive,
                    AvgRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : (float?)null,
                    IsFav = false
                })
                .ToListAsync();
            return products;
        }

        public async Task<IEnumerable<ProductOffers>> GetProductOffers(string language)
        {
            var products = await _context.Sales
                .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now
                  && s.EndDate >= DateTime.Now)
                .Include(p => p.Product)
                .ThenInclude(p => p.ProductPhotos)
                .ThenInclude(p => p.Product.ProductSizes)
                .Where( p => p.Product.IsActive && !p.Product.IsDeleted && p.Product.Language == language)
                .Select(p => new ProductOffers
                {
                    Image = p.Product.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
                    PriceAfterSale = p.Product.ProductSizes.Any()
                        ? (
                            (p.Product.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
                            - (
                                (p.Discount / 100m)
                                * (p.Product.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
                            )
                        )
                        : 0
                }).ToListAsync();
            return products;

        }

        public async Task<ProductDetailsDTO> GetProductById(string language , int id)
        {
            var products = _context.Products
                .Where(p => p.IsActive && !p.IsDeleted && p.Language == language && p.Id == id)
                .Select(p => new ProductDetailsDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.,
                });
        }

    }
}
