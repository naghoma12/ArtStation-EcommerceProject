using ArtStation.Core.Helper;

namespace ArtStation_Dashboard.ViewModels.Order
{
    public class InvoiceCompanyVM
    {
        public int OrderNum { get; set; }
        
        public decimal Total { get; set; }
        public string OrderDate { get; set; }
        public List<ProductsOFSpecificOrder> Items { get; set; }

    }
}
