using ArtStation.Core.Entities.Order;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Repository.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {


        public OrderRepository(ArtStationDbContext artStationDb)
            : base(artStationDb)
        {

        }



        public async Task<Order> GetOrderForUserAsync(int OrderId)
        {
            var order = await _context.Set<Order>().Where(O => O.Id == OrderId).Include(OI => OI.OrderItems).ThenInclude(p=>p.ProductItem.Product).ThenInclude(p=>p.ProductSizes).FirstAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string PhoneNumber)
        {
            var order = await _context.Set<Order>().Where(O => O.CustomerPhone == PhoneNumber && O.IsDeleted == false).Include(o => o.Address).ThenInclude(a=>a.Shipping).ToListAsync();
            return order;
        }

    }
}
