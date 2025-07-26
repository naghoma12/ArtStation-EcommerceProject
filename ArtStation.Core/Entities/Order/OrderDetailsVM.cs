using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public class OrderDetailsVM
    {
        public string CustomerPhone { get; set; }
        public string OrderDate { get; set; } 
        public string Status { get; set; } 
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }

       
        public Address Address { get; set; }
        public decimal SubTotal { get; set; }
   
        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    }
}
