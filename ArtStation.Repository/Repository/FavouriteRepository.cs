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
        public async Task<IEnumerable<SimpleProduct>> FavouriteProducts(string language, int userId)
        {
            var favData = await  _context.Favorites
                .Where(x => !x.IsDeleted && x.UserId == userId)
                .Include(x => x.Product)
                    .ThenInclude(p => p.ProductSizes)
                .Include(x => x.Product.ProductPhotos)
                .Include(x => x.Product.Reviews)
                .Include(x => x.Product.Sales)
                .Select(f => new
                {
                    f.Product.Id,
                    Name = language== "en"? f.Product.NameEN : f.Product.NameAR,
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
                var priceAfterSale = discount > 0 ? basePrice - (discount / 100m * basePrice) : 0;

                return new SimpleProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    PhotoUrl = p.PhotoUrl ?? "",
                    ReviewsNumber = p.Reviews.Count,
                    TotalPrice = basePrice,
                    Discount = discount,
                    IsSale = discount > 0,
                    PriceAfterSale = priceAfterSale,
                    AvgRating = p.Reviews.Any() ? (float?)p.Reviews.Average(r => r.Rating) : 0,
                    IsFav = true
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
