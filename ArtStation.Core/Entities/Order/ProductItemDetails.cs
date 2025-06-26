using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public class ProductItemDetails
    {
        public ProductItemDetails()
        {

        }

        public ProductItemDetails(int productId, string photo, string name, decimal price, decimal? priceAfterSale, string? color, string? size, string? flavour)
        {
            ProductId = productId;
            Photo = photo;
           
            Name = name;
            Price = price;
            PriceAfterSale = priceAfterSale;
            Color = color;
            Size = size;
            Flavour = flavour;
        }

        public int ProductId { get; set; }
        public string Photo { get; set; }
       
        public string Name { get; set; }

      
        public decimal Price { get; set; }
        public decimal? PriceAfterSale { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Flavour { get; set; }



    }
}
