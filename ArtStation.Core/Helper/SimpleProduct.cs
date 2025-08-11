using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class SimpleProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ReviewsNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? PriceAfterSale { get; set; }
        public bool IsSale { get; set; }

        public int? Discount { get; set; }
        public string PhotoUrl { get; set; }
        public float? AvgRating { get; set; }
        public int StockCount { get; set; }
        public string Brand { get; set; }
        public List<ForWhomDTO> ForWhom { get; set; } = new List<ForWhomDTO>();
        public bool IsFav { get; set; } = false;
    }
}
