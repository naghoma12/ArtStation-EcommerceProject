﻿using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Order;
using ArtStation.Core.Helper;
using ArtStation.Core.Helper.Order;
using ArtStation_Dashboard.ViewModels;
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
        Task<PagedResult<Order>> GetOrdersDashboardAsync(int page, int pageSize);
        Task<OrderInvoiceDto> GetOrderWithDetailsDashboardAsync(int id);


    }
}
