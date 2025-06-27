using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public class OrderItem:BaseEntity
    {
        public OrderItem()
        {

        }

        public OrderItem(ProductItemDetails productItem, int quantity, int traderId)
        {
            ProductItem = productItem;
           
            Quantity = quantity;
            TraderId = traderId;

        }

        public ProductItemDetails ProductItem { get; set; }
        public int OrderId { get; set; }
        public OrderItemStatus OrderItemStatus { get; set; } = OrderItemStatus.Placed;
      
        public decimal TotalPrice { get; set; }

        public int TraderId { get; set; }
        public int Quantity { get; set; }
    }

}
