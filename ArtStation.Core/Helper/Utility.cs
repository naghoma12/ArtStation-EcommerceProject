using ArtStation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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

            var priceAfterSale = discount > 0 ? basePrice - (discount / 100m * basePrice) : 0;

            return new SimpleProduct
            {
                Id = p.Id,
                Name = language == "en" ? p.NameEN : p.NameAR,
                PhotoUrl = p.ProductPhotos.Select(ph => ph.Photo).FirstOrDefault() ?? "",
                ReviewsNumber = p.Reviews.Count,
                TotalPrice = basePrice,
                Discount = discount,
                IsSale = discount > 0,
                PriceAfterSale = priceAfterSale,
                AvgRating = p.Reviews.Any() ? (float?)p.Reviews.Average(r => r.Rating) : 0,
                IsFav = userId.HasValue && p.Favourites.Any(f => f.UserId == userId.Value)
            };
        }


        public static int? CheckToken(string? token )
        {
            int? userId = null;
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);


                userId = int.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                              ?? jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value);
                return userId;
            }
            return userId;
        }
    }
}
