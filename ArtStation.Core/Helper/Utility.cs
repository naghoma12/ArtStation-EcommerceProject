using ArtStation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public static class Utility
    {
        public static SimpleProduct MapToSimpleProduct(Product p, int? userId, string language)
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
                Name = language == "en" ? p.NameEN : p.NameAR,
                PhotoUrl = p.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
                ReviewsNumber = p.Reviews.Count,
                TotalPrice = basePrice,
                PriceAfterSale = priceAfterSale,
                IsActive = p.IsActive,
                AvgRating = p.Reviews.Any() ? (float?)p.Reviews.Average(r => r.Rating) : 0,
                IsFav = userId.HasValue && p.Favourites.Any(f => f.UserId == userId.Value)
            };
        }
    }
}
