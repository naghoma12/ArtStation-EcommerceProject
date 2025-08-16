using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Helper.AiDtos;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using ArtStation_Dashboard.ViewModels;
using AutoMapper;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using static System.Net.WebRequestMethods;

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

            var products = rawProducts.Select(p => Utility.MapToSimpleProduct(p, userId, language));

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

            var products = rawProducts.Select(p => Utility.MapToSimpleProduct(p, userId, language));

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

            var products = rawProducts.Select(p => Utility.MapToSimpleProduct(p, userId, language));

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
                    Image = s.Product.ProductPhotos.Select(ph => string.IsNullOrEmpty(ph.Photo) ? null :
                $"http://artstationdashboard.runasp.net//Uploads//Products/{ph.Photo}").FirstOrDefault() ?? "",
                    PriceAfterSale = priceAfterSale,
                    StockCount = s.Product.StockCount

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
                .ThenInclude(p => p.AppUser)
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
                Images = product.ProductPhotos.Select(ph => $"http://artstationdashboard.runasp.net//Uploads//Products/{ph.Photo}")
                .ToList(),
                AvgRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                ReviewsNumber = product.Reviews.Count,
                StockCount = product.StockCount,
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
                ShippingDetails = language == "en" ? product.ShippingDetailsEN : product.ShippingDetailsAR,
                DeliveredOn = language == "en" ? product.DeliveredOnEN : product.DeliveredOnAR,
                Reviews = product.Reviews.Select(r =>
                    new ReviewDTO
                    {
                        Id = r.Id,
                        Comment = r.Comment,
                        Rating = r.Rating,
                        UserName = r.AppUser?.FullName ?? "Unknown",
                        UserImage = string.IsNullOrEmpty(r.AppUser.Image) ? null :
                        $"http://artstation.runasp.net//Uploads//Users/{r.AppUser?.Image}",
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
                .Select(p => Utility.MapToSimpleProduct(p, userId, language))
                .ToList();

            return matchedProducts;
        }

        public async Task<ProductWithPriceDto> GetProductWithPrice(int productId, int sizeId, int quantity)
        {
            var product = await _context.Products
               .Where(p => p.IsActive && !p.IsDeleted && p.Id == productId)
               .Include(p => p.Sales)
               .Include(p => p.ProductSizes)
               .FirstOrDefaultAsync();

            if (product == null)
            {
                return new ProductWithPriceDto
                {
                    ErrorMessage = "المنتج غير موجود"
                };
            }

         
            if (product == null)
                return null;

            var activeSale = product.Sales
                .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            var discount = activeSale?.Discount ?? 0;
            var size = product.ProductSizes.Where(s => s.Id == sizeId).FirstOrDefault();
            var priceAfterSale = size.Price - (size.Price * discount / 100m);
            if (product.StockCount < quantity)
            {
                return new ProductWithPriceDto
                {
                    product = product,
                    Price = size.Price,
                    PriceAfterSale = priceAfterSale,
                    UserId = product.UserId,
                    ErrorMessage = $"الكمية المطلوبة غير متوفرة. المتاح فقط: {product.StockCount}"
                };
            }

            // لو الكمية متاحة
            product.SellersCount += quantity;
            product.StockCount -= quantity;
            await _context.SaveChangesAsync();

            return new ProductWithPriceDto()
            {
                product = product,
                Price = size.Price,
                PriceAfterSale = priceAfterSale,
                UserId = product.UserId


            };

        }

        public async Task<ProductsOFSpecificOrder> GetProductsOfSpecificOrder(int productId, int sizeId, int? flavourId, int? ColorId, string lang = "en")
        {
            var product = await _context.Products
               .Where(p => p.Id == productId)
               .Include(p => p.Sales)
               .Include(p => p.ProductSizes)
               .Include(p => p.ProductColors)
               .Include(p => p.ProductFlavours)
                .Include(p => p.ProductPhotos)
               .FirstOrDefaultAsync();

            if (product == null)
                return null;

            var activeSale = product.Sales
                .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            var discount = activeSale?.Discount ?? 0;
            var size = product.ProductSizes.Where(s => s.Id == sizeId).FirstOrDefault();

            var color = product.ProductColors.Where(s => s.Id == ColorId).FirstOrDefault();
            var flavour = product.ProductFlavours.Where(s => s.Id == flavourId).FirstOrDefault();
            var priceAfterSale = size.Price - (size.Price * discount / 100m);
            return new ProductsOFSpecificOrder()
            {
                ProductId = product.Id,
                ProductName = lang == "en" ? product.NameEN : product.NameAR,
                //OriginalPrice = size.Price,
                
                PhotoUrl = $"http://artstationdashboard.runasp.net//Uploads//Products/{product.ProductPhotos.FirstOrDefault()?.Photo}",
                Size = lang == "en" ? size.SizeEN : size.SizeAR,
                Color = color == null ? null : (lang == "en" ? color.NameEN : color.NameAR),
                Flavour = flavour == null ? null : (lang == "en" ? flavour.NameEN : flavour.NameAR),
                //PriceAfterSale = priceAfterSale,


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



            var relatedProducts = _context.Products
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


        public async Task<IEnumerable<SimpleProduct>> FilterProducts(List<SimpleProduct> products, int? minPriceRange, int? maxPriceRange, string? brand, bool men, bool women, bool kids, bool offer)
        {
            if (minPriceRange.HasValue && maxPriceRange.HasValue)
            {
                products = products
                    .Where(p => p.TotalPrice >= (decimal)minPriceRange && p.TotalPrice <= (decimal)maxPriceRange)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(brand))
            {
                products = products
                    .Where(p => p.Brand.ToLower() == brand.ToLower())
                    .ToList();
            }
            if (men)
            {
                products = products
                     .Where(p => p.ForWhom.Any(f => f.ForWhom == ForWhom.Men.ToString()
                     || f.ForWhom == "رجال"))
                     .ToList();
            }
            if (women)
            {
                products = products
                     .Where(p => p.ForWhom.Any(f => f.ForWhom == ForWhom.Women.ToString()
                     || f.ForWhom == "نساء"))
                     .ToList();
            }
            if (kids)
            {
                products = products
                     .Where(p => p.ForWhom.Any(f => f.ForWhom == ForWhom.Kids.ToString()
                     || f.ForWhom == "أطفال"))
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

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return _context.Products
                .Where(p => p.IsActive && !p.IsDeleted)
                .Include(p => p.ProductPhotos)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsNoTracking();
        }
        public async Task<IEnumerable<Product>> GetTraderProducts(int userId)
        {
            return _context.Products
                .Where(p => p.IsActive && !p.IsDeleted
                && p.User.Id == userId)
                .Include(p => p.ProductPhotos)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsNoTracking();
        }
        public async Task<ProductDetailsVM> GetProductDetails(int id, string language)
        {
            var now = DateTime.Now;

            return await _context.Products
                .Where(p => p.Id == id )
                .Select(p => new ProductDetailsVM
                {
                    Id = p.Id,
                    Name = language == "en" ? p.NameEN : p.NameAR,
                    Description = language == "en" ? p.DescriptionEN : p.DescriptionAR,
                    ShippingDetails = language == "en" ? p.ShippingDetailsEN : p.ShippingDetailsAR,
                    DeliveredOn = language == "en" ? p.DeliveredOnEN : p.DeliveredOnAR,
                    Brand = language == "en" ? p.BrandEN : p.BrandAR,
                    Category = language == "en" ? p.Category.NameEN : p.Category.NameAR,
                    Trader = p.User.FullName,
                    StockCount = p.StockCount,
                    Images = p.ProductPhotos.Select(i => i.Photo).ToList(),
                    Colors = p.ProductColors.Select(c => language == "en" ? c.NameEN : c.NameAR).ToList(),

                    Sizes = p.ProductSizes.Select(s => new SizesDTO
                    {
                        Id = s.Id,
                        Size = language == "en" ? s.SizeEN : s.SizeAR,
                        Price = s.Price,
                        PriceAfterSale = p.Sales
                            .Where(sale => sale.IsActive && !sale.IsDeleted &&
                                           sale.StartDate <= now && sale.EndDate >= now)
                            .Select(sale => s.Price - (s.Price * sale.Discount / 100m))
                            .FirstOrDefault()  // returns discounted price if exists, or 0
                    }).ToList(),

                    Flavours = p.ProductFlavours.Select(f => language == "en" ? f.NameEN : f.NameAR).ToList(),
                    ForWhoms = p.ForWhoms.Select(f => language == "en" ? f.ForWhomEN : f.ForWhomAR).ToList()
                })
                .FirstOrDefaultAsync();
        }


        public async Task<Product> GetProductAsync(int id)
        {
            return await _context.Products
                .Where(x => x.Id == id && !x.IsDeleted && x.IsActive)
                .Include(x => x.ProductPhotos)
                .Include(x => x.ProductSizes)
                .Include(x => x.ProductColors)
                .Include(x => x.ProductFlavours)
                .Include(x => x.Sales)
                .Include(x => x.ForWhoms)
                .FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<Product>> GetInActiveProducts()
        {
            return _context.Products
                .Where(p => !p.IsActive && !p.IsDeleted)
                .Include(p => p.ProductPhotos)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsNoTracking();
        }
        public async Task<IEnumerable<Product>> GetDeletedProducts()
        {
            return _context.Products
                .Where(p => p.IsActive && p.IsDeleted)
                .Include(p => p.ProductPhotos)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsNoTracking();
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive && !p.IsDeleted)
                .Include(p => p.ProductPhotos)
                .Include(p => p.ProductSizes)
                .Include(p => p.Sales)
                .Include(p => p.Category)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetCompanyProducts(int traderId)
        {
            return await _context.Products
                    .Where(p=>p.UserId==traderId )
                    .ToListAsync();
        }
    }
}
