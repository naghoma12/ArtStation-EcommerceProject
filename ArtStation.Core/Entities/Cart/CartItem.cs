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
       
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? FlavourId { get; set; }
        public int Quantity { get; set; }
    }
}
