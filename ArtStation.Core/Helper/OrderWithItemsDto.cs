using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class OrderWithItemsDto
    {
        public int OrderId { get; set; }
        public List<ProductsOFSpecificOrder> Items { get; set; }
    }
}
