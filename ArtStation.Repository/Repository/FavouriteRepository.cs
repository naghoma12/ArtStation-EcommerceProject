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
    public class FavouriteRepository : GenericRepository<Favourite>, IFavouriteRepository
    {
        private readonly ArtStationDbContext _context;

        public FavouriteRepository(ArtStationDbContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<SimpleProduct>> FavouriteProducts(string language, int UserId)
        //{
        //    var favProducts = await _context.Favorites
        //        .Where(x => !x.IsDeleted && x.UserId ==  UserId
        //        && x.Product.Language == language)
        //        .Include(x => x.Product)
        //        .Select(p => new SimpleProduct
        //        {
        //            Id = p.Product.Id,
        //            Name = p.Product.Name,
        //            PhotoUrl = p.Product.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
        //            ReviewsNumber = p.Product.Reviews.Count(),
        //            TotalPrice = p.Product.ProductSizes.Min(x => (decimal?)x.Price) ?? 0,
        //            PriceAfterSale = p.Product.ProductSizes.Any()
        //        ? (
        //            (p.Product.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
        //            - (
        //                (p.Product.Sales
        //                    .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
        //                    .Any()
        //                    ? (
        //                        (p.Product.Sales
        //                            .Where(s => s.IsActive && !s.IsDeleted && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
        //                            .OrderByDescending(s => s.Id)
        //                            .Select(s => (int?)s.Discount)
        //                            .FirstOrDefault() ?? 0)
        //                    )
        //                    : 0
        //                ) / 100m
        //                * (p.Product.ProductSizes.Min(x => (decimal?)x.Price) ?? 0)
        //            )
        //        )
        //        : 0,
        //            IsActive = p.Product.IsActive,
        //            AvgRating = p.Product.Reviews.Any() ? p.Product.Reviews.Average(r => r.Rating) : (float?)null,
        //            IsFav = p.Product.Favourites.Any(f => f.UserId == UserId)
        //        })
        //        .ToListAsync();

        //    return favProducts;

        //}

        public async Task<IEnumerable<SimpleProduct>> FavouriteProducts(string language, int userId)
        {
            var favData = await _context.Favorites
                .Where(x => !x.IsDeleted && x.UserId == userId && x.Product.Language == language)
                .Include(x => x.Product)
                    .ThenInclude(p => p.ProductSizes)
                .Include(x => x.Product.ProductPhotos)
                .Include(x => x.Product.Reviews)
                .Include(x => x.Product.Sales)
                .Include(x => x.Product.Favourites)
                .Select(f => new
                {
                    f.Product.Id,
                    f.Product.Name,
                    PhotoUrl = f.Product.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault(),
                    Reviews = f.Product.Reviews,
                    Sizes = f.Product.ProductSizes,
                    Sales = f.Product.Sales,
                    f.Product.IsActive,
                    f.Product.Favourites
                })
                .ToListAsync();

            var result = favData.Select(p =>
            {
                var basePrice = p.Sizes.Min(s => (decimal?)s.Price) ?? 0;
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
                    PhotoUrl = p.PhotoUrl ?? "",
                    ReviewsNumber = p.Reviews.Count,
                    TotalPrice = basePrice,
                    PriceAfterSale = priceAfterSale,
                    IsActive = p.IsActive,
                    AvgRating = p.Reviews.Any() ? (float?)p.Reviews.Average(r => r.Rating) : 0,
                    IsFav = p.Favourites.Any(f => f.UserId == userId)
                };
            });

            return result;
        }
        public async Task<Favourite> GetFavoriteAsync(int ProductId, int UserId)
        {
            return await _context.Favorites
                .Where(x => !x.IsDeleted  
            && !x.Product.IsDeleted && x.Product.IsActive 
            && x.ProductId == ProductId 
            && x.UserId == UserId)
                .FirstOrDefaultAsync();
        }

        
    }
}
