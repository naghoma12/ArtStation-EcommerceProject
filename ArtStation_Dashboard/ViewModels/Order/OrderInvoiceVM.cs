using ArtStation.Core.Helper;
using System.Runtime.CompilerServices;
using ArtStation.Core.Entities.Order;
using ArtStation.Core.Entities.Identity;

namespace ArtStation_Dashboard.ViewModels.Order
{
    public class OrderInvoiceVM
    {
       
        public OrderDetailsVM Order { get; set; }
        
        public List<ProductsOFSpecificOrder> Items { get; set; }
    }
}
