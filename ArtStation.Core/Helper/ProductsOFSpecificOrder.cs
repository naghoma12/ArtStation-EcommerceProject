using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class ProductsOFSpecificOrder
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        //public decimal  OriginalPrice { get; set; }
        //public decimal PriceAfterSale { get; set; }
        public string PhotoUrl { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Flavour { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
