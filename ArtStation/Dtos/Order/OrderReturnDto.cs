using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Order;

namespace ArtStation.Dtos.Order
{
    public class OrderReturnDto
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; } 
        public string OrderStatus { get; set; } 
        public ArtStation.Core.Entities.Cart.DeliveryAddress Address { get; set; }
        public decimal SubTotal { get; set; }
        public int ShippingCost { get; set; }
        public decimal Total { get; set; }
    }
}
