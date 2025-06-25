using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using FuzzySharp;
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

        private SimpleProduct MapToSimpleProduct(Product p, int? userId)
        {
            var basePrice = p.ProductSizes.Min(x => (decimal?)x.Price) ?? 0;
            var activeSale = p.Sales
                .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();
            var discount = activeSale?.Discount ?? 0;
            var priceAfterSale = basePrice - (discount / 100m * basePrice);

            return new SimpleProduct
            {
                Id = p.Id,
                Name = p.Name,
                PhotoUrl = p.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
                ReviewsNumber = p.Reviews.Count,
                TotalPrice = basePrice,
                PriceAfterSale = priceAfterSale,
                IsActive = p.IsActive,
                AvgRating = p.Reviews.Any() ? (float?)p.Reviews.Average(r => r.Rating) : 0,
                IsFav = userId.HasValue && p.Favourites.Any(f => f.UserId == userId.Value)
            };
        }
        public async Task<IEnumerable<SimpleProduct>> GetAllProducts(string language, int? userId = null)
        {
            var rawProducts = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted && p.Language == language)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.Sales)
                .ToListAsync();

            var products = rawProducts.Select(p => MapToSimpleProduct(p, userId));

            return products;
        }

        public async Task<IEnumerable<SimpleProduct>> GetBestSellerProducts(string language, int? userId = null)
        {
            var rawProducts = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted && p.Language == language)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.Sales)
                .OrderByDescending(p => p.SellersCount)
                .ToListAsync();

            var products = rawProducts.Select(p => MapToSimpleProduct(p, userId));

            return products;
        }

        public async Task<IEnumerable<SimpleProduct>> GetNewProducts(string language, int? userId = null)
        {
            var rawProducts = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted && p.Language == language)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.Sales)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            var products = rawProducts.Select(p => MapToSimpleProduct(p, userId));

            return products;
        }

        public async Task<IEnumerable<ProductOffers>> GetProductOffers(string language)
        {
            var sales = await _context.Sales
                .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                .Include(s => s.Product)
                    .ThenInclude(p => p.ProductPhotos)
                .Include(s => s.Product)
                    .ThenInclude(p => p.ProductSizes)
                .Where(s => s.Product != null && s.Product.IsActive && !s.Product.IsDeleted && s.Product.Language == language)
                .ToListAsync();

            var offers = sales.Select(s =>
            {
                var basePrice = s.Product.ProductSizes.Min(x => (decimal?)x.Price) ?? 0;
                var discount = s.Discount;
                var priceAfterSale = basePrice - (discount / 100m * basePrice);

                return new ProductOffers
                {
                    Image = s.Product.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
                    PriceAfterSale = priceAfterSale,
                    
                };
            });

            return offers;
        }

        public async Task<ProductDetailsDTO> GetProductById(string language, int id, int? userId = null)
        {
            var product = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted && p.Language == language && p.Id == id)
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .FirstOrDefaultAsync();

            if (product == null)
                return null;

            var activeSale = product.Sales
                .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            var discount = activeSale?.Discount ?? 0;

            return new ProductDetailsDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Images = product.ProductPhotos.Select(ph => ph.Photo).ToList(),
                AvgRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                ReviewsNumber = product.Reviews.Count,
                Colors = product.ProductColors.Select(c => new ColorsDTO
                {
                    ColorName = c.Name,
                    HexCode = c.HexCode
                }).ToList(),
                Sizes = product.ProductSizes.Select(s => new SizesDTO
                {
                    Size = s.Size,
                    Price = s.Price,
                    PriceAfterSale = s.Price - (s.Price * discount / 100m)
                }).ToList(),
                Flavours = product.ProductFlavours.Select(f => f.Name).ToList(),
                ShippingDetails = product.ShippingDetails,
                DeliveredMinDate = product.DeliveredMinDate,
                DeliveredMaxDate = product.DeliveredMaxDate,
                Reviews = product.Reviews.ToList(),
                IsFav = userId.HasValue && product.Favourites.Any(f => f.UserId == userId.Value)
            };
        }

        public async Task<IEnumerable<SimpleProduct>> SearchByProductName(string proName, string language, int? userId = null)
        {
            var dbProducts = await _context.Products
                .Where(p => !p.IsDeleted && p.IsActive && p.Language == language &&
                            p.Name.ToLower().Contains(proName.ToLower())) 
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .ToListAsync();

            int similarityThreshold = 70;
            var matchedProducts = dbProducts
                .Where(p =>
                    Fuzz.Ratio(p.Name, proName) > similarityThreshold ||
                    p.Name.Trim().ToLower().Contains(proName.Trim().ToLower()))
                .Select(p => MapToSimpleProduct(p, userId)).ToList();

            return matchedProducts;
        }
    }
}
