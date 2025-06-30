using ArtStation.Core.Entities.Order;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Order>> GetUserOrdersAsync(string PhoneNumber);
        public Task<Order> GetOrderForUserAsync(int OrderId);
        //public Task<Order> CancelOrderForUserAsync(int OrderId);
        //public Task<Order> ReOrderForUserAsync(int OrderId);
        //public Task<Order> DeleteOrderForUserAsync(int OrderId);
        //public Task<OrderItem> GetOrderItemAsync(int orderitemId);

    }
}
