using ArtStation.Core.Entities.Order;
using ArtStation.Core.Helper;
using ArtStation.Core.Helper.Order;
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
        public Task<OrderWithItemsDto> GetOrderForUserAsync(int OrderId);

        //Dashboard
        public Task<IEnumerable<Order>> GetOrdersAsync(int page, int pageSize);
        public Task<OrderInvoiceDto> GetOrderWithDetailsAsync(int id);

    }
}
