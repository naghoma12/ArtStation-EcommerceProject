using ArtStation.Core.Entities.Order;
using ArtStation.Core.Helper;
using ArtStation.Core.Helper.Order;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using Google.Apis.Util;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = ArtStation.Core.Entities.Order.Order;

namespace ArtStation.Repository.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly IProductRepository _productRepository;

        public OrderRepository(ArtStationDbContext artStationDb,IProductRepository productRepository)
            : base(artStationDb)
        {
            _productRepository = productRepository;
        }

        public async Task<OrderWithItemsDto> GetOrderForUserAsync(int orderId)
        {
            var orders = await _context.Set<Order>()
       .Where(o => o.Id == orderId)
        .Include(o => o.OrderItems)
           
       .FirstOrDefaultAsync();
            var lang=CultureInfo.CurrentUICulture.Name;
            List<ProductsOFSpecificOrder> items = new List<ProductsOFSpecificOrder>();
            foreach (var item in orders.OrderItems)
            {
                var orderItem=await _productRepository.GetProductsOfSpecificOrder(item.ProductItem.ProductId, (int)item.ProductItem.SizeId, item.ProductItem.FlavourId, item.ProductItem.ColorId,lang);
                orderItem.Quantity = item.Quantity;
                orderItem.SubTotal = item.TotalPrice;

                
                
                items.Add(orderItem);
            }



            return new OrderWithItemsDto
            {
                OrderId = orders.Id,
              
                Items = items
            };
           
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string PhoneNumber)
        {
            var order = await _context.Set<Order>().Where(O => O.CustomerPhone == PhoneNumber && O.IsDeleted == false).Include(o => o.Address).ThenInclude(a=>a.Shipping).ToListAsync();
            return order;
        }

        //Dashboard 

        public async Task<IEnumerable<Order>> GetOrdersAsync(int page, int pageSize)
        {
            return await _context.Set<Order>()
                .Where(o => o.IsDeleted == false)
                 .OrderByDescending(o => o.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
               
       
      

        }

        public async Task<OrderInvoiceDto> GetOrderWithDetailsAsync(int id)
        {

            var order = await _context.Set<Order>()
                 .Where(o => o.Id == id && o.Id == id)
                 .Include(o => o.OrderItems)
                 .Include(a => a.Address)
                 .ThenInclude(a => a.Shipping)

                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.IsActive == true);


            var lang = CultureInfo.CurrentUICulture.Name;
            List<ProductsOFSpecificOrder> items = new List<ProductsOFSpecificOrder>();
            foreach (var item in order.OrderItems)
            {
                var orderItem = await _productRepository.GetProductsOfSpecificOrder(item.ProductItem.ProductId, (int)item.ProductItem.SizeId, item.ProductItem.FlavourId, item.ProductItem.ColorId, lang);
                orderItem.Quantity = item.Quantity;
                orderItem.SubTotal = item.TotalPrice;



                items.Add(orderItem);
            }
            return new OrderInvoiceDto
            {
                Order=order,
                Items = items,
            };
        }


    }
}
