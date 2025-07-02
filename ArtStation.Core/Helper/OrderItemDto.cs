using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public  class OrderItemDto
    {
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Flavour  { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
