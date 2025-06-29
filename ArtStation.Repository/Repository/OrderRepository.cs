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



        //public async Task<Order> GetOrderForUserAsync(int OrderId)
        //{
        //    var order = await _context.Set<Order>().Where(O => O.Id == OrderId).Include(OI => OI.OrderItems).Include(o => o.Shipping).FirstAsync();
        //    return order;
        //}

        //public async Task<IEnumerable<Order>> GetUserOrdersAsync(string Email)
        //{
        //    var order = await _context.Set<Order>().Where(O => O.CustomerEmail == Email && O.IsDeleted == false).Include(o => o.Shipping).Include(o => o.OrderItems).ToListAsync();
        //    return order;
        //}

    }
    }
