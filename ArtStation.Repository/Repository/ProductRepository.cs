using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public ProductRepository(ArtStationDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

      
        public async Task<IEnumerable<SimpleProduct>> GetAllProducts(string language, int? userId = null)
        {
            var rawProducts = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.Sales)
                .Include(p => p.Favourites)
                .Include(p => p.ForWhoms)
                .ToListAsync();

            var products = rawProducts.Select(p => Utility.MapToSimpleProduct(p, userId , language));

            return products;
        }

        public async Task<IEnumerable<SimpleProduct>> GetBestSellerProducts(string language, int? userId = null)
        {
            var rawProducts = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.Sales)
                .Include(p => p.Favourites)
                .Include(p => p.ForWhoms)
                .OrderByDescending(p => p.SellersCount)
                .ToListAsync();

            var products = rawProducts.Select(p => Utility.MapToSimpleProduct(p, userId , language));

            return products;
        }

        public async Task<IEnumerable<SimpleProduct>> GetNewProducts(string language, int? userId = null)
        {
            var rawProducts = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.Sales)
                .Include(p => p.Favourites)
                .Include(p => p.ForWhoms)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            var products = rawProducts.Select(p => Utility.MapToSimpleProduct(p, userId , language));

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
                .Where(s => s.Product != null && s.Product.IsActive && !s.Product.IsDeleted)
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
                .Where(p => p.IsActive && !p.IsDeleted && p.Id == id)
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductFlavours)
                .Include(p => p.Sales)
                .Include(p => p.Favourites)
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
                Name = language == "en" ? product.NameEN : product.NameAR,
                Description = language == "en" ? product.DescriptionEN : product.DescriptionAR,
                Images = product.ProductPhotos.Select(ph => ph.Photo).ToList(),
                AvgRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                ReviewsNumber = product.Reviews.Count,
                Colors = product.ProductColors.Select(c => new ColorsDTO
                {
                    Id = c.Id,
                    ColorName = language == "en" ? c.NameEN : c.NameAR,
                    HexCode = c.HexCode
                }).ToList(),
                Sizes = product.ProductSizes.Select(s => new SizesDTO
                {
                    Id = s.Id,
                    Size = language == "en" ? s.SizeEN : s.SizeAR,
                    Price = s.Price,
                    PriceAfterSale = s.Price - (s.Price * discount / 100m)
                }).ToList(),
                Flavours = product.ProductFlavours.Select(f => new FlavourDTO
                {
                   Id = f.Id,
                    Name = language == "en" ? f.NameEN : f.NameAR
                }).ToList(),
                ShippingDetails = language == "en" ?  product.ShippingDetailsEN : product.ShippingDetailsAR,
                 DeliveredOn = language == "en" ? product.DeliveredOnEN : product.DeliveredOnAR,
                Reviews = product.Reviews.Select( r=> 
                    new ReviewDTO
                    {
                        Id = r.Id,
                        Comment = r.Comment,
                        Rating = r.Rating,
                        UserName = r.AppUser?.UserName ?? "Unknown",
                        UserImage = r.AppUser?.Image ?? "",
                        LikesCount = r.LikesCount
                    }).ToList(),
                IsFav = userId.HasValue && product.Favourites.Any(f => f.UserId == userId.Value)
            };
        }
        public async Task<IEnumerable<SimpleProduct>> SearchByProductName(string proName, string language, int? userId = null)
        {
            var lowerProName = proName.Trim().ToLower();

            var dbProducts = await _context.Products
                .Where(p => !p.IsDeleted && p.IsActive &&
                            (p.NameEN.ToLower().Contains(lowerProName) || p.NameAR.ToLower().Contains(lowerProName)))
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .Include(p => p.Favourites)
                .Include(p => p.ForWhoms)
                .ToListAsync();

            int similarityThreshold = 70;

            var matchedProducts = dbProducts
                .Where(p =>
                    Fuzz.Ratio(p.NameEN ?? "", proName) > similarityThreshold ||
                    Fuzz.Ratio(p.NameAR ?? "", proName) > similarityThreshold ||
                    (p.NameEN.Trim().ToLower().Contains(lowerProName)) ||
                    (p.NameAR.Trim().ToLower().Contains(lowerProName)))
                .Select(p => Utility.MapToSimpleProduct(p, userId,language))
                .ToList();

            return matchedProducts;
        }

        public async Task<ProductWithPriceDto> GetProductWithPrice(int productId,int sizeId )
        {
            var product = await _context.Products
               .Where(p => p.IsActive && !p.IsDeleted && p.Id == productId)
               .Include(p => p.Sales)
               .Include(p => p.ProductSizes)
               .FirstOrDefaultAsync();

            if (product == null)
                return null;

            var activeSale = product.Sales
                .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            var discount = activeSale?.Discount ?? 0;
            var size = product.ProductSizes.Where(s => s.Id == sizeId).FirstOrDefault();
            var priceAfterSale=size.Price -(size.Price * discount / 100m);
            return new ProductWithPriceDto()
            {
                Product=product,
                Price=size.Price,
                PriceAfterSale = priceAfterSale,


            };
            
        }

        public async Task<IEnumerable<SimpleProduct>> GetRelatedProducts(int productId, string language, int? userId = null)
        {
            var product = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted && p.Id == productId)
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .Include(x => x.Category)
                .Include(p => p.Favourites)
                .Include(p => p.ForWhoms)
                .FirstOrDefaultAsync();



            var relatedProducts =  _context.Products
                .Where(p => p.IsActive && !p.IsDeleted 
                && (p.Category.NameAR == product.Category.NameAR || p.Category.NameEN == product.Category.NameEN) 
                || (p.BrandAR == product.BrandAR || p.BrandEN == product.BrandEN))
                .Include(p => p.ProductPhotos)
                .Include(p => p.Reviews)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .Include(p => p.Favourites)
                .Include(p => p.ForWhoms)
;

            var simpleProducts = relatedProducts.Where(x => x.Id != productId)
                .Select(p => Utility.MapToSimpleProduct(p, userId, language))
                .Take(5);

            return simpleProducts;
        }

        public async Task<IEnumerable<AIProducts>> GetAIProducts(string language)
        {
            var products = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted)
                .Select(p => new AIProducts
                {
                    Id = p.Id,
                    Name = language == "en" ? p.NameEN : p.NameAR
                })
                .ToListAsync();

            return products;
        }
        public async Task<IEnumerable<BrandDTO>> GetBrands(string language)
        {

           var brands = await _context.Products
                .Where(p => p.IsActive && !p.IsDeleted)
                .Select(p => new BrandDTO
                {
                    Name = language == "en" ? p.BrandEN : p.BrandAR
                })
                .Distinct()
                .ToListAsync();

            return brands;
        }
    

        public async Task<IEnumerable<SimpleProduct>> FilterProducts(List<SimpleProduct> products,int? minPriceRange, int? maxPriceRange , string? brand , bool men , bool women , bool kids , bool offer )
        {
            if(minPriceRange.HasValue && maxPriceRange.HasValue)
            {
                products = products
                    .Where(p => p.TotalPrice >= (decimal)minPriceRange && p.TotalPrice <= (decimal)maxPriceRange)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(brand))
            {
                products = products
                    .Where(p => p.Brand == brand)
                    .ToList();
            }
            if (men)
            {
                products = products
                     .Where(p => p.ForWhom.Any(f => f.ForWhom == ForWhom.Men.ToString() 
                     || f.ForWhom == ForWhom.رجال.ToString()))
                     .ToList();
            }
            if (women)
            {
                products = products
                     .Where(p => p.ForWhom.Any(f => f.ForWhom == ForWhom.Women.ToString()
                     || f.ForWhom == ForWhom.نساء.ToString()))
                     .ToList();
            }
            if (kids)
            {
                products = products
                     .Where(p => p.ForWhom.Any(f => f.ForWhom == ForWhom.Kids.ToString()
                     || f.ForWhom == ForWhom.أطفال.ToString()))
                     .ToList();
            }
            if (offer)
            {
                products = products
                    .Where(p => p.IsSale)
                    .ToList();
            }
            return products;
        }

    }
}
