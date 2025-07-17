using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Order;
using ArtStation.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Services.Contract
{
    public interface IOrderService
    {
        Task<(Order? order, string? redirectUrl, string? paymentToken)> CreateOrderAsync(AppUser user, string CartId,int addressId,string paymentType);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string PhoneNumber);
        Task<OrderWithItemsDto> GetOrderForUserAsync(int orderid);


        //Dashboard
        Task<IEnumerable<Order>> GetOrdersDashboardAsync();
        Task<Order> GetOrderWithDetailsDashboardAsync();


    }
}
