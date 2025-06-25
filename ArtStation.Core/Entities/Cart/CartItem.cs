using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Cart
{
    public class CartItem
    {
        public string ItemId { get; set; } = Guid.NewGuid().ToString();
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    
        public string PhotoUrl { get; set; }
      
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal? Price { get; set; }
       
        public decimal? PriceAfterSale { get; set; } = 0.0m;

      
        public int Quantity { get; set; }
    }
}
