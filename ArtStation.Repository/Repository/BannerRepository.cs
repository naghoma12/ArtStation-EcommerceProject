using ArtStation.Core.Entities;
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
    public class BannerRepository : GenericRepository<Banner>, IBannerRepository
    {
        public BannerRepository(ArtStationDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<Banner>> GetAllBannersSortedAsync()
        {
            var banners = await _context.Banners.Where(b => b.IsActive == true)
                 .OrderBy(b => b.OrderBanner)
                 .ToListAsync();
            return banners;
        }
    }
}
