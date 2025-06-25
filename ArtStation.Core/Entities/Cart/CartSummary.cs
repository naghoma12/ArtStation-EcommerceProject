using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Cart
{
    public class CartSummary
    {
        public int TotalItems { get; set; }
        public decimal TotalPriceBeforeDiscount { get; set; }
        public decimal ShippingPrice { get; set; } = 0.0M;
        public decimal TotalPriceAfterDiscount { get; set; }
        public decimal FinalTotal { get; set; }
    }
}
