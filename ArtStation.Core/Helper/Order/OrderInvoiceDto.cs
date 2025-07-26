using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper.Order
{
    public class OrderInvoiceDto
    {
        public ArtStation.Core.Entities.Order.Order Order { get; set; }

        public List<ProductsOFSpecificOrder> Items { get; set; }
    }
}
