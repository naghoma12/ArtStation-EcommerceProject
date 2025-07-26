using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Order;
using ArtStation.Core.Entities.Payment;

namespace ArtStation_Dashboard.ViewModels.Order
{
    public class OrderVM
    {

        public int OrderNum { get; set; }
        public string CustomerPhone { get; set; }
        public string OrderDate { get; set; } 
        public string Status { get; set; } 
        public string PaymentStatus { get; set; } 
        public string PaymentMethod { get; set; }

        //public Address Address { get; set; }
        public decimal SubTotal { get; set; }
        
      

    }
}
