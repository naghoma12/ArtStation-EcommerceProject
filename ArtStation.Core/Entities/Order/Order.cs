using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public class Order
    {
        public Order()
        {

        }
        public Order(string customerEmail, Address shippingAddress, decimal subTotal, ICollection<OrderItem> orderItems, Shipping shippingCost)
        {
            CustomerEmail = customerEmail;
            ShippingAddress = shippingAddress;
            SubTotal = subTotal;
            OrderItems = orderItems;
            Shipping = shippingCost;

        }

        public string CustomerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Placed;
        //public OrderReady StatusReady { get; set; } = OrderReady.NotReady;
        public Address ShippingAddress { get; set; }
        public int? ShippingId { get; set; }
        public Shipping Shipping { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GetTotal() => SubTotal + Shipping.Cost;
        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public string PaymentId { get; set; } = "";

    }
}
