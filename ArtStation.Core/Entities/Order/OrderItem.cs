using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public class OrderItem
    {
        public OrderItem()
        {

        }

        public OrderItem(ProductItemDetails productItem, int quantity, string traderId)
        {
            ProductItem = productItem;
           
            Quantity = quantity;
            TraderId = traderId;

        }

        public ProductItemDetails ProductItem { get; set; }
        public int OrderId { get; set; }
        public OrderItemStatus OrderItemStatus { get; set; } = OrderItemStatus.Placed;
        private decimal totalprice;

        public decimal TotalPrice
        {
            get { return totalprice; }
            set { totalprice = value; }
        }

        public string TraderId { get; set; }
        public int Quantity { get; set; }
    }

}
