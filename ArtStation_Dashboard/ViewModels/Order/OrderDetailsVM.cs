using ArtStation.Core.Entities.Order;
using ArtStation.Core.Entities.Payment;

namespace ArtStation_Dashboard.ViewModels.Order
{
    public class OrderDetailsVM
    {
        public int OrderNum { get; set; }
        public string CustomerPhone { get; set; }
        public decimal DeliveryCost { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string? AddressDetails { get; set; }
        public decimal SubTotal { get; set; }
        public string OrderDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; } 
        public string PaymentStatus { get; set; } 
    }
}
