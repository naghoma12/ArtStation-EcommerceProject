using ArtStation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class ProductWithPriceDto
    {
        public Product Product { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterSale { get; set; }
        public int UserId { get; set; }
    }
}
