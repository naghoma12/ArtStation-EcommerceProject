using ArtStation.Core.Entities.Order;
using ArtStation.Core.Helper;
using ArtStation.Core.Helper.Order;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using ArtStation_Dashboard.ViewModels;
using Google.Apis.Util;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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

        public async Task<Order> GetOrderWithItemAsync(int OrderId)
        {
            var order = await _context.Set<Order>()
               .Where(o => o.Id == OrderId)
               .Include(o => o.OrderItems)
               .FirstOrDefaultAsync();
            return order;
        }

        /// For Company Dashboard 
        public async Task<PagedResult<Order>> GetOrdersForSpecificCompanyAsync(
     int traderId, int page, int pageSize, string statusFilter)
        {
            // Try to parse the filter into the enum
            OrderItemStatus? parsedStatus = null;
            if (!string.IsNullOrEmpty(statusFilter) &&
                Enum.TryParse(statusFilter, out OrderItemStatus statusEnum))
            {
                parsedStatus = statusEnum;
            }

            // Build query
            var ordersQuery = _context.Set<Order>()
                .Where(order => order.OrderItems.Any(item =>
                    item.TraderId == traderId && item.OrderItemStatus == parsedStatus)).Include(order=>order.OrderItems.Where(i=>i.TraderId==traderId));


            // Count after filtering
            var totalItems = await ordersQuery.CountAsync();

            // Apply pagination and ordering
            var items = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return paged result
            return new PagedResult<Order>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber =page,
                PageSize=pageSize,
                TotalPages= (int)Math.Ceiling((double)totalItems / pageSize)
            };
        }
        public decimal GetDailyMoneyCount()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return _context.Set<Order>()
                .Where(x => x.CreatedDate >= today && x.CreatedDate < tomorrow)
                .Sum(x => x.SubTotal);
        }
        public int GetDailyOrdersCount()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return _context.Set<Order>()
                .Where(x => x.CreatedDate >= today && x.CreatedDate < tomorrow)
                .Count();
        }
        public int GetCompanyOrdersCount(string phoneNumber)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return _context.Set<Order>()
                .Where(x => x.CreatedDate >= today && x.CreatedDate < tomorrow
                && x.CustomerPhone == phoneNumber)
                .Count();
        }
        public decimal GetDailyCompanyMoneyCount(string phoneNumber)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return _context.Set<Order>()
                .Where(x => x.CreatedDate >= today && x.CreatedDate < tomorrow
                && x.CustomerPhone == phoneNumber)
                .Sum(x => x.SubTotal);
        }

        public int GetYesterdayOrdersCount()
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            return _context.Set<Order>()
                .Where(x => x.CreatedDate >= yesterday
                            && x.CreatedDate < today)
                .Count();
        }

        //Get Order With Items Of Specific Trader
        public async Task<Order> GetOrderWithItemsForSpecificCompanyAsync(int OrderId, int traderid)
        {
            var order = await _context.Set<Order>()
               //.Where(order => order.Id == OrderId)
               .Include(oi => oi.OrderItems.Where(i => i.TraderId == traderid))
               .FirstOrDefaultAsync();
            return order;
        }
        public int GetYesterdayOrdersCount(string phoneNumber)
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            return _context.Set<Order>()
                .Where(x => x.CreatedDate >= yesterday
                            && x.CreatedDate < today
                            && x.CustomerPhone == phoneNumber)
                .Count();
        }

        public List<int> GetWeeklySales()
        {
            var today = DateTime.Today;
            var weekStart = today.AddDays(-6); // 7 days including today

            // Group orders by date and sum daily sales amount
            var salesByDay = _context.Set<Order>()
                .Where(o => o.CreatedDate >= weekStart && o.CreatedDate <= today)
                .GroupBy(o => o.CreatedDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalSales = g.Count() // or Count() if you want order count
                })
                .ToList();

            // Ensure all 7 days are included, even if sales = 0
            var result = Enumerable.Range(0, 7)
                .Select(i => {
                    var date = weekStart.AddDays(i);
                    var dayData = salesByDay.FirstOrDefault(x => x.Date == date);
                    return dayData?.TotalSales ?? 0;
                })
                .ToList();

            return result;
        }
        public List<int> GetWeeklySales(string phoneNumber)
        {
            var today = DateTime.Today;
            var weekStart = today.AddDays(-6); // 7 days including today

            // Group orders by date and sum daily sales amount
            var salesByDay = _context.Set<Order>()
                .Where(o => o.CreatedDate >= weekStart && o.CreatedDate <= today
                && o.CustomerPhone == phoneNumber)
                .GroupBy(o => o.CreatedDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalSales = g.Count() // or Count() if you want order count
                })
                .ToList();

            // Ensure all 7 days are included, even if sales = 0
            var result = Enumerable.Range(0, 7)
                .Select(i => {
                    var date = weekStart.AddDays(i);
                    var dayData = salesByDay.FirstOrDefault(x => x.Date == date);
                    return dayData?.TotalSales ?? 0;
                })
                .ToList();

            return result;
        }
           

        public async Task<OrderInvoiceDto> GetInvoiceForTraderAsync(int OrderId, int TraderId)
        {
           var order = await _context.Set<Order>()
             .Where(order => order.Id == OrderId)
             .Include(oi => oi.OrderItems.Where(i => i.TraderId == TraderId))
             .Include(o => o.Address)
             .FirstOrDefaultAsync();
            var lang = CultureInfo.CurrentUICulture.Name;
            List<ProductsOFSpecificOrder> items = new List<ProductsOFSpecificOrder>();
            foreach (var item in order.OrderItems)
            {
                var orderItem = await _productRepository.GetProductsOfSpecificOrder(item.ProductItem.ProductId, (int)item.ProductItem.SizeId, item.ProductItem.FlavourId, item.ProductItem.ColorId, lang);
                orderItem.Quantity = item.Quantity;
                orderItem.SubTotal = item.TotalPrice;
                orderItem.OrderItemStatus = item.OrderItemStatus.ToString();


                items.Add(orderItem);
            }
            return new OrderInvoiceDto
            {
                Order = order,
                Items = items,
            };
          
        }

        public async Task<string> ReadyOrderForCompanyAsync(int OrderId, int traderid)
        {
            var order =  await _context.Set<Order>()
            .Where(order => order.Id == OrderId)
            .Include(oi => oi.OrderItems.Where(i => i.TraderId == traderid))
            .FirstOrDefaultAsync();

            foreach (var item in order.OrderItems)
            {
                item.OrderItemStatus = OrderItemStatus.Shipped;
            }

            var row=_context.SaveChanges();
            if (row >0)
            {
                return "Ready Order";
            }
            return "Not Ready Order";
        }

     
    }
}
